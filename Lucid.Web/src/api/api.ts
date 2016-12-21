import * as request from 'superagent';

export const baseUrl = 'http://localhost:5000';

let accessToken: string | undefined = '';
export const setAccessToken = (token: string) => accessToken = token;

export interface IArea {
  name: string;
  description?: string;
  rooms?: IRoom[];
  id: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface IRoom {
  name: string;
  description?: string;
  areaId: number;
  area?: IArea;
  northRoom?: IRoom;
  northRoomId?: number;
  eastRoom?: IRoom;
  eastRoomId?: number;
  southRoom?: IRoom;
  southRoomId?: number;
  westRoom?: IRoom;
  westRoomId?: number;
  upRoom?: IRoom;
  upRoomId?: number;
  downRoom?: IRoom;
  downRoomId?: number;
  id: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface IAreaCreationRequest {
  name: string;
  description?: string;
}

export interface IAreaUpdateRequest {
  id: number;
  name: string;
  description?: string;
}

export interface IRoomCreationRequest {
  areaId: number;
  name: string;
  description?: string;
  northRoomId?: number;
  eastRoomId?: number;
  southRoomId?: number;
  westRoomId?: number;
  upRoomId?: number;
  downRoomId?: number;
}

export interface IRoomUpdateRequest {
  id: number;
  areaId: number;
  name: string;
  description?: string;
  northRoomId?: number;
  eastRoomId?: number;
  southRoomId?: number;
  westRoomId?: number;
  upRoomId?: number;
  downRoomId?: number;
}

export interface IUser {
  name: string;
  hashedPassword: string;
  currentRoomId?: number;
  currentRoom?: IRoom;
  id: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface IRequestParams {
  method: string;
  url: string;
  queryParameters?: { [key: string]: string | boolean | number };
  body?: Object;
}

const executeRequest = async <T>(params: IRequestParams) => {
  return new Promise<T>((resolve, reject) => {
    let req = request(params.method, `http://localhost:5000${params.url}`)
      .set('Content-Type', 'application/json');

    if (params.queryParameters) { req = req.query(params.queryParameters); }
    if (params.body) { req.send(params.body); }

    if (accessToken) {
      req.set('Authorization', `Bearer ${accessToken}`);
    }

    req.end((error: any, response: any) => {
      if (error || !response.ok) {
        if (response && response.body && response.body.ExceptionMessage) {
          reject(response.body.ExceptionMessage);
          return;
        }

        reject(error);
      } else {
        resolve(response.body);
      }
    });
  });
};

export const ApiAreasGet = () => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Areas`
  };
  return executeRequest<IArea[]>(requestParams);
};
export interface IApiAreasPostParams {
  request: IAreaCreationRequest;
}

export const ApiAreasPost = (params: IApiAreasPostParams) => {
  const requestParams: IRequestParams = {
    method: 'POST',
    url: `/api/Areas`
  };
  requestParams.body = params.request;
  return executeRequest<IArea>(requestParams);
};
export interface IApiAreasPatchParams {
  request: IAreaUpdateRequest;
}

export const ApiAreasPatch = (params: IApiAreasPatchParams) => {
  const requestParams: IRequestParams = {
    method: 'PATCH',
    url: `/api/Areas`
  };
  requestParams.body = params.request;
  return executeRequest<IArea>(requestParams);
};
export interface IApiAreasByIdGetParams {
  id: number;
}

export const ApiAreasByIdGet = (params: IApiAreasByIdGetParams) => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Areas/${params.id}`
  };
  return executeRequest<IArea>(requestParams);
};
export interface IApiAreasByIdRoomsGetParams {
  id: number;
}

export const ApiAreasByIdRoomsGet = (params: IApiAreasByIdRoomsGetParams) => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Areas/${params.id}/rooms`
  };
  return executeRequest<IRoom[]>(requestParams);
};
export interface IApiRoomsPostParams {
  request: IRoomCreationRequest;
}

export const ApiRoomsPost = (params: IApiRoomsPostParams) => {
  const requestParams: IRequestParams = {
    method: 'POST',
    url: `/api/Rooms`
  };
  requestParams.body = params.request;
  return executeRequest<IRoom>(requestParams);
};
export interface IApiRoomsPatchParams {
  request: IRoomUpdateRequest;
}

export const ApiRoomsPatch = (params: IApiRoomsPatchParams) => {
  const requestParams: IRequestParams = {
    method: 'PATCH',
    url: `/api/Rooms`
  };
  requestParams.body = params.request;
  return executeRequest<IRoom>(requestParams);
};
export const ApiUsersGet = () => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Users`
  };
  return executeRequest<IUser[]>(requestParams);
};
export interface IApiUsersByIdGetParams {
  id: number;
}

export const ApiUsersByIdGet = (params: IApiUsersByIdGetParams) => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Users/${params.id}`
  };
  return executeRequest<IUser>(requestParams);
};
