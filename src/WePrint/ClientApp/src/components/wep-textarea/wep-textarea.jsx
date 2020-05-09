import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './wep-textarea.scss';

function WepTextarea(props) {
  const { name, id, value, placeholder = '', handleChange, error, register } = props;
  const className = classNames('wep-textarea', { 'wep-textarea--error': error });
  if (handleChange) {
    return (
      <div className="wep-textarea__wrapper">
        <textarea
          className={className}
          name={name}
          id={id}
          value={value}
          placeholder={placeholder}
          onChange={handleChange}
        />
      </div>
    );
  }

  return (
    <div className="wep-textarea__wrapper">
      <textarea
        ref={register}
        className={className}
        name={name}
        id={id}
        defaultValue={value}
        placeholder={placeholder}
      />
    </div>
  );
}

WepTextarea.propTypes = {
  name: PropTypes.string,
  id: PropTypes.string,
  value: PropTypes.string,
  placeholder: PropTypes.string,
  handleChange: PropTypes.func,
  error: PropTypes.bool,
  register: PropTypes.func,
};

export default WepTextarea;
