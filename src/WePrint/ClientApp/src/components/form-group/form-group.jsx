import React from 'react';
import PropTypes from 'prop-types';

import './form-group.scss';

export const FormType = {
  PRIMARY: 'primary',
  SUBFORM: 'sub-form',
};

function FormGroup(props) {
  const { title, help, children, type = FormType.PRIMARY } = props;
  return (
    <div className={`form-group form-group--${type}`}>
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
};

FormGroup.Type = FormType;

export default FormGroup;
