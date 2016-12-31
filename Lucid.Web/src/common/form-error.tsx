import * as React from 'react';
import * as colors from 'material-ui/styles/colors';

export interface IFormErrorProps {
  message?: string;
}

export const FormError = (props: IFormErrorProps) => {
  if (!props.message) {
    return <div />;
  }

  return (
    <div style={{ padding: '1em 2em', textAlign: 'center', color: colors.pink300 }}>
      {props.message}
    </div>
  );
};
