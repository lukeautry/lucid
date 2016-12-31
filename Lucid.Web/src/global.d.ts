declare namespace __MaterialUI {
  interface TextFieldProps {
    autoFocus?: boolean;
    spellCheck?: boolean;
    autoComplete?: 'off';
    required?: boolean;
    onTouchTap?: Function;
    onClick?: Function;
  }

  interface TimePickerProps {
    required?: boolean;
  }

  interface AutoCompleteProps {
    autoFocus?: boolean;
  }

  interface SelectFieldProps {
    onTouchTap?: Function;
    onClick?: Function;
  }

  namespace List {
    interface ListItemProps {
      containerElement?: any;
    }
  }

  namespace Switches {
    interface CheckboxProps {
      onTouchTap?: Function;
    }
  }

  namespace Table {
    interface TableRowColumnProps {
      title?: string;
    }
  }
}

declare module 'formsy-react' {
  interface ValidationErrors {
    [key: string]: string;
  }

  // This is declared for a reference to Formsy.Mixin in FormsyFormComponent.ts
  var Mixin: any;

  interface FormProps {
    className?: string;
    mapping?: Function;
    onSuccess?: Function;
    onError?: Function;
    onSubmit?: Function;
    onValidSubmit?: Function;
    onInvalidSubmit?: Function;
    onSubmitted?: Function;
    onValid?: Function;
    onInvalid?: Function;
    onChange?: Function;
    validationErrors?: ValidationErrors;
    preventExternalValidation?: boolean;
  }

  class Form extends React.Component<FormProps, any> {
    submit(): void;
  }
}

interface EventTarget {
  value: any;
}

declare module 'formsy-material-ui' {
  interface ValidationProps {
    validations?: string;
    validationError?: string;
    required?: boolean;
    updateImmediately?: boolean;
    validationErrors?: any;
    formNoValidate?: boolean;
  }

  interface FormsyCheckbox {

  }

  interface FormsyDate {

  }
  interface FormsyRadio {

  }

  interface FormsyRadioGroup {

  }

  interface FormsySelect {

  }

  class FormsyText extends React.Component<__MaterialUI.TextFieldProps & ValidationProps, {}> {

  }

  interface FormsyTime {

  }

  interface FormsyToggle {

  }
}
