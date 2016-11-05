import * as React from 'react';
import { Link } from 'react-router';

export const NavigationLink = (props: { to: string, text: string }) => (
  <li className='nav-link'>
    <Link to={props.to} activeClassName='active'>{props.text}</Link>
  </li>
);
