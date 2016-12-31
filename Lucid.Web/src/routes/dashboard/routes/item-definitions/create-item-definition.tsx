import * as React from 'react';
import { observer } from 'mobx-react';
import { observable } from 'mobx';
import { IItemDefinition, ApiItemDefinitionsPost } from 'api/api';
import { Dialog, LinearProgress, TextField, FlatButton } from 'material-ui';

interface ICreateItemDefinitionProps {
    onCreate: (itemDefinition: IItemDefinition) => void;
    open: boolean;
    onClose: () => void;
}

@observer
export class CreateItemDefinition extends React.Component<ICreateItemDefinitionProps, {}> {
    @observable private isSaving = false;
    @observable private name = '';
    @observable private description = '';

    public render() {
        return <Dialog open={this.props.open} title='Create Item Definition' onRequestClose={this.props.onClose}>
            {this.isSaving ? <LinearProgress mode='indeterminate' /> : null}
            <form onSubmit={this.submit}>
                <TextField name='item-name' hintText='Item Name' floatingLabelText='Item Name' type='text' onChange={this.onNameChange} required fullWidth />
                <TextField name='item-description' hintText='Item Description' floatingLabelText='Item Description' type='text' onChange={this.onDescriptionChange} required fullWidth />
                <div className='dialog-footer'>
                    <FlatButton label='Create Item Definition' primary={true} type='submit' />
                </div>
            </form>
        </Dialog>;
    }

    private onNameChange = (event: { target: { value: string } }) => this.name = event.target.value;
    private onDescriptionChange = (event: { target: { value: string } }) => this.description = event.target.value;

    private submit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        const itemDefinition = await ApiItemDefinitionsPost({
            request: {
                name: this.name,
                description: this.description
            }
        });

        this.props.onCreate(itemDefinition);
        this.props.onClose();
    }
}
