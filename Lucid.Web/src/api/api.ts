import * as request from 'superagent';

export interface Area {
  name: string;
  description?: string;
  id: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface AreaCreationRequest {
  name: string;
  description?: string;
}

export interface AreaUpdateRequest {
  id: number;
  name: string;
  description?: string;
}

export interface Room {
  name: string;
  areaId: number;
  description?: string;
  northRoomId?: number;
  eastRoomId?: number;
  southRoomId?: number;
  westRoomId?: number;
  upRoomId?: number;
  downRoomId?: number;
  id: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface RoomCreationRequest {
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

export interface RoomUpdateRequest {
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

export interface User {
  name?: string;
  hashedPassword?: string;
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

const executeRequest = <T>(params: IRequestParams) => {
  return new Promise<T>((resolve, reject) => {
    let req = request(params.method, `http://localhost:5000${params.url}`)
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

export const ApiAreasGet = () => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Areas`
  };
  return executeRequest<Area[]>(requestParams);
};

export interface IApiAreasPostParams {
  request: AreaCreationRequest;
}

export const ApiAreasPost = (params: IApiAreasPostParams) => {
  const requestParams: IRequestParams = {
    method: 'POST',
    url: `/api/Areas`
  };
  requestParams.body = params.request;
  return executeRequest<Area>(requestParams);
};

export interface IApiAreasPatchParams {
  request: AreaUpdateRequest;
}

export const ApiAreasPatch = (params: IApiAreasPatchParams) => {
  const requestParams: IRequestParams = {
    method: 'PATCH',
    url: `/api/Areas`
  };
  requestParams.body = params.request;
  return executeRequest<Area>(requestParams);
};

export interface IApiAreasByIdGetParams {
  id: number;
}

export const ApiAreasByIdGet = (params: IApiAreasByIdGetParams) => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Areas/${params.id}`
  };
  return executeRequest<Area>(requestParams);
};

export interface IApiAreasByIdRoomsGetParams {
  id: number;
}

export const ApiAreasByIdRoomsGet = (params: IApiAreasByIdRoomsGetParams) => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Areas/${params.id}/rooms`
  };
  return executeRequest<Room[]>(requestParams);
};

export interface IApiRoomsPostParams {
  request: RoomCreationRequest;
}

export const ApiRoomsPost = (params: IApiRoomsPostParams) => {
  const requestParams: IRequestParams = {
    method: 'POST',
    url: `/api/Rooms`
  };
  requestParams.body = params.request;
  return executeRequest<Room>(requestParams);
};

export interface IApiRoomsPatchParams {
  request: RoomUpdateRequest;
}

export const ApiRoomsPatch = (params: IApiRoomsPatchParams) => {
  const requestParams: IRequestParams = {
    method: 'PATCH',
    url: `/api/Rooms`
  };
  requestParams.body = params.request;
  return executeRequest<Room>(requestParams);
};

export const ApiUsersGet = () => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Users`
  };
  return executeRequest<User[]>(requestParams);
};

export interface IApiUsersByIdGetParams {
  id: number;
}

export const ApiUsersByIdGet = (params: IApiUsersByIdGetParams) => {
  const requestParams: IRequestParams = {
    method: 'GET',
    url: `/api/Users/${params.id}`
  };
  return executeRequest<User>(requestParams);
};

