import * as React from 'react';
import { IItemDefinition, ApiItemsPost } from 'api/api';
import { Dialog, SelectField, MenuItem, TextField, FlatButton } from 'material-ui';
import { observable } from 'mobx';
import { observer } from 'mobx-react';
import { FormError } from 'common/form-error';

interface ICreateItemInstanceProps {
  open: boolean;
  onClose: () => void;
  itemDefinition: IItemDefinition;
}

@observer
export class CreateItemInstance extends React.Component<ICreateItemInstanceProps, {}> {
  @observable private parentObjectId: number;
  @observable private parentObjectType = 2;
  @observable private errorMessage = '';

  public render() {
    return <Dialog open={this.props.open} onRequestClose={this.props.onClose} title={`Create instance of ${this.props.itemDefinition.name}`}>
      <FormError message={this.errorMessage} />
      <form onSubmit={this.createItem}>
        <SelectField onChange={this.onObjectTypeChange} value={this.parentObjectType} floatingLabelText='Parent Object Type' fullWidth>
          <MenuItem value={2} primaryText='User' />
          <MenuItem value={1} primaryText='Room' />
        </SelectField>
        <TextField required floatingLabelText='Parent Object ID' floatingLabelFixed={true} fullWidth type='number' onChange={this.onParentObjectIdChange} />
        <div className='dialog-footer'>
          <FlatButton label='Create Instance' primary={true} type='submit' />
        </div>
      </form>
    </Dialog>
  }

  private onObjectTypeChange = (event: any, index: number, value: number) => this.parentObjectType = value;
  private onParentObjectIdChange = (event: { target: { value: string } }) => this.parentObjectId = parseInt(event.target.value, 10);

  private createItem = async (event: React.FormEvent<any>) => {
    event.preventDefault();

    try {
      await ApiItemsPost({
        request: {
          itemDefinitionId: this.props.itemDefinition.id,
          parentObjectId: this.parentObjectId,
          parentObjectType: this.parentObjectType
        }
      })
      this.props.onClose();
    } catch (err) {
      this.errorMessage = err.message;
    }
  };
}
