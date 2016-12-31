import * as React from 'react';
import { ApiAreasGet, IArea } from 'api/api';
import LinearProgress from 'material-ui/LinearProgress';
import * as colors from 'material-ui/styles/colors';
import { observable } from 'mobx';
import { observer } from 'mobx-react';
import { AddArea } from './add-area';
import { Table, TableBody, TableHeader, TableHeaderColumn, TableRow, TableRowColumn } from 'material-ui/Table';
import FloatingActionButton from 'material-ui/FloatingActionButton';
import ContentAdd from 'material-ui/svg-icons/content/add';
import { getViewAreaPath } from './view-area';
import { Link } from 'react-router';

export const areasPath = '/areas';

@observer
export class Areas extends React.Component<{}, {}> {
  @observable private areas: IArea[];
  @observable private isAddingNewArea = false;

  public componentDidMount() {
    ApiAreasGet()
      .then(areas => this.areas = areas);
  }

  public render() {
    return (
      this.areas
        ? <div className='content'>
          <h1>
            <span>Areas</span>
          </h1>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHeaderColumn colSpan={2} style={{ textAlign: 'right' }}>
                  <FloatingActionButton mini={true} secondary={true} onClick={this.addNewArea}>
                    <ContentAdd />
                  </FloatingActionButton>
                </TableHeaderColumn>
              </TableRow>
              <TableRow>
                <TableHeaderColumn>ID</TableHeaderColumn>
                <TableHeaderColumn>Name</TableHeaderColumn>
              </TableRow>
            </TableHeader>
            <TableBody>
              {this.areas.map((area, index) => (
                <TableRow key={index}>
                  <TableRowColumn>{area.id}</TableRowColumn>
                  <TableRowColumn>
                    <Link className='link' to={getViewAreaPath(area.id)}>{area.name}</Link>
                  </TableRowColumn>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <AddArea isOpen={this.isAddingNewArea} onClose={this.closeAddNewArea} onSave={this.onSave} />
        </div>
        : <LinearProgress mode='indeterminate' color={colors.lightBlue300} />
    );
  }

  private addNewArea = () => this.isAddingNewArea = true;
  private closeAddNewArea = () => this.isAddingNewArea = false;
  private onSave = (area: IArea) => {
    this.areas.push(area);
    this.closeAddNewArea();
  };
}
