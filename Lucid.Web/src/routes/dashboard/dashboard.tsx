import * as React from 'react';
import { AppBar, Drawer, MenuItem } from 'material-ui';
import getMuiTheme from 'material-ui/styles/getMuiTheme';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import * as colors from 'material-ui/styles/colors';
import * as spacing from 'material-ui/styles/spacing';
import { fade } from 'material-ui/utils/colorManipulator';
import { Link } from 'react-router';
import { areasPath } from './routes/areas/areas';
import { usersPath } from './routes/users/users';
import { observable } from 'mobx';
import { observer } from 'mobx-react';

export interface IDashboardProps {
  children?: React.ReactNode;
}

const theme = getMuiTheme({
  spacing: spacing.default,
  fontFamily: 'Roboto, sans-serif',
  palette: {
    primary1Color: colors.lightBlue400,
    primary2Color: colors.fullWhite,
    primary3Color: colors.fullWhite,
    accent1Color: colors.lightBlueA400,
    accent2Color: colors.lightBlueA200,
    accent3Color: colors.lightBlueA100,
    textColor: colors.fullWhite,
    alternateTextColor: colors.fullWhite,
    canvasColor: '#303030',
    borderColor: fade(colors.fullWhite, 0.3),
    disabledColor: fade(colors.fullWhite, 0.3),
    pickerHeaderColor: fade(colors.fullWhite, 0.12),
    clockCircleColor: fade(colors.fullWhite, 0.12)
  }
});

@observer
export class Dashboard extends React.Component<IDashboardProps, {}> {
  @observable private drawerOpen = false;

  public render() {
    return (
      <MuiThemeProvider muiTheme={theme}>
        <div className='dashboard'>
          <AppBar title='Lucid MUD' iconClassNameRight='muidocs-icon-navigation-expand-more' onLeftIconButtonTouchTap={this.onMenuClick} />
          <Drawer open={this.drawerOpen} onRequestChange={this.onRequestChange} docked={false}>
            <Link to={usersPath} activeClassName='active' onClick={this.onRequestChange}>
              <MenuItem className='menu-item' primaryText='Users' />
            </Link>
            <Link to={areasPath} activeClassName='active' onClick={this.onRequestChange}>
              <MenuItem className='menu-item' primaryText='Areas' />
            </Link>
          </Drawer>
          {this.props.children}
        </div >
      </MuiThemeProvider >
    );
  }

  private onMenuClick = () => {
    this.drawerOpen = !this.drawerOpen;
  };

  private onRequestChange = () => {
    this.drawerOpen = false;
  };
}
