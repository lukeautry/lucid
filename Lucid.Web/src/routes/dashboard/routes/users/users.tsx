import './users.scss';
import * as React from 'react';
import { ApiUsersGet, User } from '../../../../api/api';
import { UserList } from './user-list';
import LinearProgress from 'material-ui/LinearProgress';
import * as colors from 'material-ui/styles/colors';
import { observable } from 'mobx';
import { observer } from 'mobx-react';

export const usersPath = '/users';

@observer
export class Users extends React.Component<{}, {}> {
  @observable private users: User[];
  @observable private isLoading = true;

  public componentDidMount() {
    ApiUsersGet().then(users => {
      this.users = users;
      this.isLoading = false;
    });
  }

  public render() {
    return (
      <div>
        {this.isLoading
          ?
          <LinearProgress mode='indeterminate' color={colors.lightBlue300} />
          :
          <div className='content users'>
            <h1>Users</h1>
            <UserList users={this.users} />
          </div>}
      </div>
    );
  }
}
