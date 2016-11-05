import './index.scss';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Dashboard } from './routes/dashboard/dashboard';
import { Users, usersPath } from './routes/dashboard/routes/users/users';
import { Areas, areasPath } from './routes/dashboard/routes/areas/areas';
import { ViewArea, viewAreaPath } from './routes/dashboard/routes/areas/view-area';
import { Router, Route, IndexRoute, hashHistory } from 'react-router';
import * as injectTapEventPlugin from 'react-tap-event-plugin';

injectTapEventPlugin();

ReactDOM.render((
  <Router history={hashHistory}>
    <Route path='/' component={Dashboard}>
      <Route path={usersPath} component={Users} />
      <Route path={areasPath}>
        <IndexRoute component={Areas} />
        <Route path={viewAreaPath} component={ViewArea} />
      </Route>
    </Route>
  </Router>
), document.getElementById('app'));
