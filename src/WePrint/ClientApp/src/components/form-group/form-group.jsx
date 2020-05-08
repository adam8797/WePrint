import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';

import './form-group.scss';

export const FormType = {
  PRIMARY: 'primary',
  SUBFORM: 'sub-form',
};

export const FormStyle = {
  CONDENSED: 'condensed',
};

function FormGroup(props) {
  const { title, help, children, type = FormType.PRIMARY, styles = [] } = props;
  const formClasses = classNames(
    'form-group',
    `form-group--${type}`,
    styles.map(s => `form-group--${s}`)
  );
  return (
    <div className={formClasses}>
      <div className="form-group__header">
        <div className="form-group__title">{title}</div>
        <div className="form-group__help">{help}</div>
      </div>
      <div className="form-group__content">{children}</div>
    </div>
  );
}

FormGroup.propTypes = {
  title: PropTypes.string.isRequired,
  children: PropTypes.node.isRequired,
  help: PropTypes.string,
  type: PropTypes.string,
  styles: PropTypes.arrayOf(PropTypes.string),
};

FormGroup.Type = FormType;
FormGroup.Style = FormStyle;

export default FormGroup;
