import { FormsyText } from 'formsy-material-ui';
import { IRoom, IRoomUpdateRequest, ApiRoomsPatch } from '../../../../api/api';
import { observable } from 'mobx';
import { observer } from 'mobx-react';
import * as Formsy from 'formsy-react';
import * as React from 'react';
import Dialog from 'material-ui/Dialog';
import FlatButton from 'material-ui/FlatButton';

export interface IEditRoomProps {
  room: IRoom;
  isOpen: boolean;
  onClose: () => void;
  onSave: (room: IRoom) => void;
}

@observer
export class EditRoom extends React.Component<IEditRoomProps, {}> {
  @observable private name = '';
  @observable private description = '';
  @observable private inProgress = false;

  constructor(public readonly props: IEditRoomProps) {
    super();
  }

  public render() {
    return (
      <Dialog open={this.props.isOpen} title={`Editing Room '${this.props.room.name}'`} onRequestClose={this.onClose}>
        <Formsy.Form onValidSubmit={this.onSubmit}>
          <div className='form-row'>
            <FormsyText
              name='room-name'
              hintText='Name'
              floatingLabelText='Name'
              floatingLabelFixed={true}
              value={this.props.room.name}
              onChange={this.onNameChange}
              required
              fullWidth
              />
          </div>
          <div className='form-row'>
            <FormsyText
              name='room-description'
              hintText='Description'
              floatingLabelText='Description'
              floatingLabelFixed={true}
              multiLine={true}
              value={this.props.room.description || ''}
              onChange={this.onDescriptionChange}
              fullWidth
              />
          </div>
          <div className='dialog-footer'>
            <FlatButton label='Cancel' primary={true} onTouchTap={this.onClose} />
            <FlatButton label='Create Room' primary={true} keyboardFocused={true} type='submit' />
          </div>
        </Formsy.Form>
      </Dialog>
    );
  }

  protected onNameChange = (event: any) => this.name = event.target.value;
  protected onDescriptionChange = (event: any) => this.description = event.target.value;

  protected onSubmit = () => {
    this.inProgress = true;
    const clonedRoom = Object.assign({}, this.props.room) as IRoomUpdateRequest;
    clonedRoom.name = this.name || this.props.room.name;
    clonedRoom.description = this.description || this.props.room.description;

    ApiRoomsPatch({
      request: clonedRoom
    })
      .then(room => {
        this.inProgress = false;
        this.props.onSave(room);
        this.onClose();
      });

    return false;
  };

  private onClose = () => {
    this.props.onClose();
  };
}
