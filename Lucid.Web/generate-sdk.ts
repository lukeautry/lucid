import * as fs from 'fs';
import * as Swagger from './swagger';
import * as handlebars from 'handlebars';
import * as request from 'superagent';

interface ITemplateView {
  models: ITemplateModel[];
  paths: ITemplatePath[];
  baseUrl: string;
}

interface ITemplateModel {
  name: string;
  properties: Array<{
    propertyName: string;
    propertyType: string;
  }>;
}

interface ITemplatePath {
  operations: ITemplateOperation[];
}

interface ITemplateOperation {
  id: string;
  method: string;
  signature: string;
  urlTemplate: string;
  returnType: string;
  paramsInterfaceName?: string;
  parameters?: ITemplateOperationParameters[];
  queryParameters?: string[];
  bodyParameter?: string;
  hasParameters: boolean;
  hasBodyParameter: boolean;
}

interface ITemplateOperationParameters {
  parameterName: string;
  parameterType: string;
}

const getSwaggerJson = () => {
  return new Promise<Swagger.ISpec>((resolve, reject) => {
    request
      .get('http://localhost:5000/swagger/v1/swagger.json')
      .set('Accept', 'application/json')
      .end((err, res) => {
        resolve(res.body);
      });
  });
};

getSwaggerJson();

const template = handlebars.compile(`import * as request from 'superagent';

{{#models}}
export interface {{name}} {
  {{#properties}}
  {{propertyName}}: {{propertyType}};
  {{/properties}}
}

{{/models}}
export interface IRequestParams {
  method: string;
  url: string;
  queryParameters?: { [key: string]: string | boolean | number };
  body?: Object;
}

const executeRequest = <T>(params: IRequestParams) => {
  return new Promise<T>((resolve, reject) => {
    let req = request(params.method, \`{{baseUrl}}\${params.url}\`)
      .set('Content-Type', 'application/json');

    if (params.queryParameters) { req = req.query(params.queryParameters); }
    if (params.body) { req.send(params.body); }

    req.end((error: any, response: any) => {
      if (error || !response.ok) {
        reject(error);
      } else {
        resolve(response.body);
      }
    });
  });
};

{{#paths}}
{{#operations}}
{{#if hasParameters}}
export interface {{paramsInterfaceName}} {
  {{#parameters}}
  {{parameterName}}: {{parameterType}};
  {{/parameters}}
}

{{/if}}
export const {{id}} = ({{signature}}) => {
  const requestParams: IRequestParams = {
    method: '{{method}}',
    url: \`{{urlTemplate}}\`
  };
  {{#if queryParameters}}
  requestParams.queryParameters = {
  {{#queryParameters}}
  {{this}}: params.{{this}},
  {{/queryParameters}}
  };
  {{/if}}
  {{#if hasBodyParameter}}
  requestParams.body = params.{{bodyParameter}};
  {{/if}}
  return executeRequest<{{returnType}}>(requestParams);
};

{{/operations}}
{{/paths}}
`);

const getTypeFromRef = ($ref: string) => $ref.replace('#/definitions/', '');

const getPropertyTypeFromSwaggerProperty = (property: Swagger.ISchema): string => {
  if (property.type === 'integer') { return 'number'; }
  if (property.type === 'boolean') { return 'boolean'; }
  if (property.type === 'string') {
    return property.format === 'date-time' ? 'Date' : 'string';
  }

  if (property.type === 'array') {
    const items = property.items as Swagger.ISchema;
    if (!items) { throw new Error(); }

    if (items.type) {
      return `${getPropertyTypeFromSwaggerProperty(items.type)}[]`;
    }

    return `${getTypeFromRef(items.$ref as string)}[]`;
  }

  if (property.$ref) { return getTypeFromRef(property.$ref); }

  return 'any';
};

const getPropertyTypeFromSwaggerParameter = (parameter: Swagger.IBaseParameter): string => {
  const pathParameter = parameter as Swagger.IPathParameter;
  if (pathParameter.type) {
    if (pathParameter.type === 'integer') { return 'number'; }
    if (pathParameter.type === 'boolean') { return 'boolean'; }
    if (pathParameter.type === 'string') {
      return pathParameter.type === 'date-time' ? 'Date' : 'string';
    }
  }

  const bodyParameter = parameter as Swagger.IBodyParameter;
  const schema = bodyParameter.schema;
  if (schema) {
    if (schema.$ref) {
      return getTypeFromRef(schema.$ref);
    }

    if (schema.type === 'array') {
      const items = schema.items as Swagger.ISchema;
      return `${getTypeFromRef(items.$ref as string)}[]`;
    }
  }

  return 'any';
};

const getTemplateView = (swagger: Swagger.ISpec): ITemplateView => {
  const definitions = swagger.definitions;
  if (!definitions) { throw new Error(); }

  const paths = swagger.paths;
  if (!paths) { throw new Error(); }

  return {
    baseUrl: `${swagger.host}${(swagger.basePath || '').replace(/\/$/, '')}`,
    paths: Object.keys(paths).map(pathKey => {
      const methods = ['get', 'post', 'delete', 'patch', 'put', 'options', 'head'];
      const path = paths[pathKey];

      return {
        operations: Object.keys(path)
          .filter(operationKey => methods.find(m => m === operationKey))
          .map(operationKey => {
            const operation = (path as any)[operationKey] as Swagger.IOperation;
            const operationId = operation.operationId;
            if (!operationId) { throw new Error(); }

            const parameters = operation.parameters;
            const operationParameters = new Array<ITemplateOperationParameters>();

            // /api/{someParam}/{anotherParam} => /api/${someParam}/${anotherParam}
            let urlTemplate = `${pathKey}`.replace(/\{/g, '${');
            let signature = '';
            let paramsInterfaceName = '';
            let queryParameters: string[] | undefined = undefined;
            let bodyParameter: string | undefined;
            let hasBodyParameter = false;

            if (parameters && parameters.length) {
              paramsInterfaceName = `I${operationId.charAt(0).toUpperCase() + operationId.slice(1)}Params`;
              signature = `params: ${paramsInterfaceName}`;
              parameters.forEach(parameter => {
                operationParameters.push({
                  parameterName: parameter.name,
                  parameterType: getPropertyTypeFromSwaggerParameter(parameter)
                });

                if (parameter.in === 'path') {
                  urlTemplate = urlTemplate.replace(parameter.name, `params.${parameter.name}`);
                  return;
                }

                if (parameter.in === 'query') {
                  queryParameters = queryParameters || new Array<string>();
                  queryParameters.push(parameter.name);
                  return;
                }

                if (parameter.in === 'body') {
                  hasBodyParameter = true;
                  bodyParameter = parameter.name;
                }
              });
            }

            let returnType = 'void';
            if (operation.responses['200']) {
              returnType = getPropertyTypeFromSwaggerProperty(operation.responses['200'].schema as Swagger.ISchema);
            }

            return {
              id: operationId,
              method: operationKey.toUpperCase(),
              signature,
              urlTemplate,
              parameters: operationParameters,
              hasParameters: !!operationParameters.length,
              bodyParameter,
              queryParameters,
              returnType,
              paramsInterfaceName,
              hasBodyParameter
            } as ITemplateOperation;
          })
      };
    }),
    models: Object.keys(definitions).map(definitionKey => {
      const definition = definitions[definitionKey];
      if (!definition) { throw new Error(); }

      const properties = definition.properties;
      if (!properties) { throw new Error(); }

      return {
        name: definitionKey,
        properties: Object.keys(properties).map(propertyKey => {
          const property = properties[propertyKey];
          const isRequired = definition.required && definition.required.find(propertyName => propertyName === propertyKey);

          return {
            propertyName: `${propertyKey}${isRequired ? '' : '?'}`,
            propertyType: getPropertyTypeFromSwaggerProperty(property)
          };
        })
      };
    })
  };
};

getSwaggerJson()
  .then(swaggerSpec => {
    try {
      swaggerSpec.host = "http://localhost:5000";
      const compiled = template(getTemplateView(swaggerSpec));
      fs.writeFileSync('./src/api/api.ts', compiled);
      console.log('Api file generated!');
    } catch (err) {
      console.log(err);
    }
  });
