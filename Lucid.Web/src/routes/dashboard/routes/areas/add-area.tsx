import { FormsyText } from 'formsy-material-ui';
import { Area, ApiAreasPost } from '../../../../api/api';
import { observable } from 'mobx';
import { observer } from 'mobx-react';
import * as Formsy from 'formsy-react';
import * as React from 'react';
import Dialog from 'material-ui/Dialog';
import FlatButton from 'material-ui/FlatButton';

export interface IAddAreaProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (area: Area) => void;
}

@observer
export class AddArea extends React.Component<IAddAreaProps, {}> {
  @observable private name: string;
  @observable private description: string;
  @observable private inProgress: boolean;

  constructor(public readonly props: IAddAreaProps) {
    super();
    this.setDefaults();
  }

  public render() {
    return (
      <Dialog open={this.props.isOpen} title={'Create Area'} onRequestClose={this.onClose}>
        <Formsy.Form onValidSubmit={this.onSubmit}>
          <div className='form-row'>
            <FormsyText
              name='area-name'
              hintText='Name'
              floatingLabelText='Name'
              floatingLabelFixed={true}
              value={this.name}
              onChange={this.onNameChange}
              required
              />
          </div>
          <div className='form-row'>
            <FormsyText
              name='area-description'
              hintText='Description'
              floatingLabelText='Description'
              floatingLabelFixed={true}
              multiLine={true}
              value={this.description}
              onChange={this.onDescriptionChange}
              required
              />
          </div>
          <div className='dialog-footer'>
            <FlatButton label='Cancel' primary={true} onTouchTap={this.onClose} />
            <FlatButton label='Create Area' primary={true} keyboardFocused={true} type='submit' />
          </div>
        </Formsy.Form>
      </Dialog>
    );
  }

  protected onNameChange = (event: any) => this.name = event.target.value;
  protected onDescriptionChange = (event: any) => this.description = event.target.value;

  protected onSubmit = () => {
    this.inProgress = true;
    ApiAreasPost({
      request: {
        name: this.name,
        description: this.description
      }
    })
      .then(area => {
        this.props.onSave(area);
        this.onClose();
      });

    return false;
  };

  private onClose = () => {
    this.props.onClose();
    this.setDefaults();
  };

  private setDefaults() {
    this.name = '';
    this.description = '';
    this.inProgress = false;
  }
}
