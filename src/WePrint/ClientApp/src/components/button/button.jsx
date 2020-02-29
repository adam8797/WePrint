import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './button.scss';

export const ButtonType = {
  PRIMARY: 'primary',
};

export const ButtonSize = {
  SMALL: 'small',
  MEDIUM: 'medium',
  LARGE: 'large',
};

function Button(props) {
  const { children, className, type, htmlType, size, ...rest } = props;

  const buttonClass = classNames('button', className, `button--${size}`, `button--${type}`);

  return (
    // eslint-disable-next-line react/button-has-type
    <button {...rest} className={buttonClass} type={htmlType}>
      <div className="button__inner-container">
        {/* {iconName && <Icon name={iconName} className="button__icon" />} */}
        {children}
      </div>
    </button>
  );
}

Button.propTypes = {
  children: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]).isRequired,
  className: PropTypes.string,
  type: PropTypes.oneOf(Object.values(ButtonType)),
  size: PropTypes.oneOf(Object.values(ButtonSize)),
  htmlType: PropTypes.string,
};

Button.defaultProps = {
  className: '',
  type: ButtonType.PRIMARY,
  size: ButtonSize.SMALL,
  htmlType: 'button',
};

Button.Type = ButtonType;

export default Button;
