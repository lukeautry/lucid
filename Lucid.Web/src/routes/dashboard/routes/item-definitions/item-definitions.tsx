import * as React from 'react';
import {
    LinearProgress, Table, TableBody, TableHeader, TableHeaderColumn, TableRow, TableRowColumn,
    RaisedButton, IconMenu, MenuItem, IconButton
} from 'material-ui';
import { observer } from 'mobx-react';
import { observable } from 'mobx';
import { ApiItemDefinitionsGet, IItemDefinition } from 'api/api';
import { CreateItemDefinition } from './create-item-definition';
import { CreateItemInstance } from './create-item-instance';
import MoreVertIcon from 'material-ui/svg-icons/navigation/more-vert';

export const itemDefinitionsPath = '/item-definitions';

@observer
export class ItemDefinitions extends React.Component<{}, {}> {
    @observable private isLoading = true;
    @observable private itemDefinitions: IItemDefinition[];
    @observable private isCreatingItemDefinition = false;
    @observable private creatingItem?: IItemDefinition;

    constructor() {
        super();
        this.fetchItemDefinitions();
    }

    public render() {
        if (this.isLoading) {
            return <LinearProgress mode='indeterminate' />
        }

        const onIconMenuClick = (event: React.TouchEvent<any>) => event.stopPropagation();

        return <div className='content'>
            <h1>Item Definitions</h1>
            <div className='button-bar'>
                <RaisedButton label='Create Item Definition' primary={true} onTouchTap={this.onTapCreateItemDefinition} />
            </div>
            <Table>
                <TableHeader adjustForCheckbox={false} displaySelectAll={false}>
                    <TableRow>
                        <TableHeaderColumn>Item ID</TableHeaderColumn>
                        <TableHeaderColumn>Item Name</TableHeaderColumn>
                        <TableHeaderColumn>Item Description</TableHeaderColumn>
                        <TableHeaderColumn></TableHeaderColumn>
                    </TableRow>
                </TableHeader>
                <TableBody displayRowCheckbox={false}>
                    {this.itemDefinitions.map((itemDefinition, index) => {
                        const onTapCreateInstance = () => this.creatingItem = itemDefinition;

                        return <TableRow key={index}>
                            <TableRowColumn>{itemDefinition.id}</TableRowColumn>
                            <TableRowColumn>{itemDefinition.name}</TableRowColumn>
                            <TableRowColumn>{itemDefinition.description}</TableRowColumn>
                            <TableRowColumn>
                                <IconMenu iconButtonElement={<IconButton><MoreVertIcon /></IconButton>} onTouchTap={onIconMenuClick}>
                                    <MenuItem onTouchTap={onTapCreateInstance}>Create Instance</MenuItem>
                                </IconMenu>
                            </TableRowColumn>
                        </TableRow>
                    })}
                </TableBody>
            </Table>
            <CreateItemDefinition onClose={this.onCloseCreateItemDefinition} open={this.isCreatingItemDefinition} onCreate={this.onCreateItemDefinition} />
            {this.creatingItem && <CreateItemInstance open={!!this.creatingItem} itemDefinition={this.creatingItem} onClose={this.onCreateItemInstanceClose} />}
        </div>
    }

    private onCloseCreateItemDefinition = () => this.isCreatingItemDefinition = false;
    private onTapCreateItemDefinition = () => this.isCreatingItemDefinition = true;
    private onCreateItemDefinition = (itemDefinition: IItemDefinition) => this.itemDefinitions.push(itemDefinition);
    private onCreateItemInstanceClose = () => this.creatingItem = undefined;

    private async fetchItemDefinitions() {
        this.itemDefinitions = await ApiItemDefinitionsGet();
        this.isLoading = false;
    }
}
