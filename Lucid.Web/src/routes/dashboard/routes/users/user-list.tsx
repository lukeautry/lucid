import * as React from 'react';
import { User } from '../../../../api/api';
import { Table, TableBody, TableHeader, TableHeaderColumn, TableRow, TableRowColumn } from 'material-ui/Table';
import FloatingActionButton from 'material-ui/FloatingActionButton';
import ContentAdd from 'material-ui/svg-icons/content/add';

export interface IUserListProps {
  users: User[];
}

export const UserList = (props: IUserListProps) => (
  <Table>
    <TableHeader>
      <TableRow>
        <TableHeaderColumn colSpan={2} style={{ textAlign: 'right' }}>
          <FloatingActionButton mini={true} secondary={true}>
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
      {props.users.map((user, index) => (
        <TableRow key={index}>
          <TableRowColumn>{user.id}</TableRowColumn>
          <TableRowColumn>{user.name}</TableRowColumn>
        </TableRow>
      ))}
    </TableBody>
  </Table>
);
