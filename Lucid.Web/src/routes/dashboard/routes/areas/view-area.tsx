import './view-area.scss';
import * as React from 'react';
import { Area, Room, ApiAreasByIdRoomsGet, ApiAreasByIdGet, ApiRoomsPatch } from '../../../../api/api';
import LinearProgress from 'material-ui/LinearProgress';
import * as colors from 'material-ui/styles/colors';
import { observer } from 'mobx-react';
import { observable } from 'mobx';
import { EditArea } from './edit-area';
import { MapEditor } from './map-editor/map-editor';
import { AddRoom } from './add-room';

export const viewAreaPath = `/areas/:id`;
export const getViewAreaPath = (id: number) => `/areas/${id}`;

export interface IEditAreaProps {
  params: {
    id: number;
  };
}

export type Direction = 'north' | 'east' | 'south' | 'west' | 'up' | 'down';

@observer
export class ViewArea extends React.Component<IEditAreaProps, {}> {
  @observable private area: Area;
  @observable private rooms: Room[];
  @observable private isEditingArea = false;
  @observable private isAddingRoom = false;
  @observable private areaName: string;
  @observable private areaDescription: string;
  @observable private updateFn?: (newRoomId: number) => Room;

  constructor(public readonly props: IEditAreaProps) {
    super();
  }

  public componentDidMount() {
    this.loadArea();
  }

  public render() {
    return (
      <div className='content'>
        {
          this.area
            ?
            <div>
              <h1>Area:
                <span> {this.area.name}</span>
                <i className='material-icons btn' onClick={this.editAreaName} title='Edit Area Details'>mode_edit</i>
              </h1>
              <p className='well description'>{this.areaDescription}</p>
              <MapEditor area={this.area} addNewRoom={this.addNewRoom} addConnectingRoom={this.addConnectingRoom} updateRoom={this.callUpdateRoom} rooms={this.rooms}/>
              <EditArea area={this.area} onSave={this.onSave} isOpen={this.isEditingArea} onClose={this.closeEditArea} />
              <AddRoom isOpen={this.isAddingRoom} onSave={this.onAddRoom} onClose={this.closeAddRoom} areaId={this.area.id} />
            </div>
            : <LinearProgress mode='indeterminate' color={colors.lightBlue300} />
        }
      </div>
    );
  }

  protected editAreaName = () => this.isEditingArea = true;
  protected closeEditArea = () => this.isEditingArea = false;
  protected onSave = (area: Area) => {
    this.setArea(area);
    this.closeEditArea();
  };

  protected onAddRoom = (room: Room) => {
    if (this.updateFn) {
      const augmentedRoom = this.updateFn(room.id);
      this.callUpdateRoom(augmentedRoom);
      return;
    }

    this.refreshArea();
  };

  private addConnectingRoom = (updateFn: (newRoomId: number) => Room) => {
    this.updateFn = updateFn;
    this.isAddingRoom = true;
  }

  private callUpdateRoom = (room: Room) => {
    ApiRoomsPatch({ request: room })
      .then(() => this.refreshArea());
  }

  private refreshArea() {
    this.loadArea();
    this.closeAddRoom();
  }

  private setArea(area: Area) {
    this.area = area;
    this.areaName = area.name;
    this.areaDescription = area.description || '';
  }

  private addNewRoom = () => {
    this.updateFn = undefined;
    this.isAddingRoom = true;
  };

  private loadArea() {
    ApiAreasByIdGet({ id: this.props.params.id })
      .then(area => {
        this.setArea(area);
        return ApiAreasByIdRoomsGet({ id: area.id })
          .then(rooms => this.rooms = rooms);
      });
  }

  private closeAddRoom = () => this.isAddingRoom = false;
}
