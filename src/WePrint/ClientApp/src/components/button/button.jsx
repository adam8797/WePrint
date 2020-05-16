import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import ReactTooltip from 'react-tooltip';

import './button.scss';

export const ButtonType = {
  PRIMARY: 'primary',
  SUCCESS: 'success',
  DANGER: 'danger',
};

export const ButtonSize = {
  SMALL: 'small',
  MEDIUM: 'medium',
  LARGE: 'large',
};

function Button(props) {
  const {
    children,
    className,
    type,
    htmlType,
    size,
    icon,
    selected,
    outline,
    tooltip,
    tooltipType,
    ...rest
  } = props;

  const buttonClass = classNames(
    'button',
    className,
    `button--${size}`,
    `button--${type}${outline ? '--outline' : ''}`,
    `button--${selected ? 'selected' : 'deselected'}`
  );

  return (
    // eslint-disable-next-line react/button-has-type
    <button {...rest} className={buttonClass} type={htmlType}>
      <div
        className="button__inner-container"
        data-tip={tooltip || ''}
        data-type={tooltipType || 'dark'}
      >
        {icon && <FontAwesomeIcon icon={icon} className="button__icon" />}
        {children && <div className="button__title">{children}</div>}
      </div>
      <ReactTooltip />
    </button>
  );
}

Button.propTypes = {
  children: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  className: PropTypes.string,
  type: PropTypes.oneOf(Object.values(ButtonType)),
  size: PropTypes.oneOf(Object.values(ButtonSize)),
  htmlType: PropTypes.string,
  icon: PropTypes.string,
  selected: PropTypes.bool,
  outline: PropTypes.bool,
  tooltip: PropTypes.string,
  tooltipType: PropTypes.string,
};

Button.defaultProps = {
  children: '',
  className: '',
  type: ButtonType.PRIMARY,
  size: ButtonSize.MEDIUM,
  htmlType: 'button',
  icon: '',
  selected: true,
  tooltip: null,
  tooltipType: 'dark',
};

Button.Type = ButtonType;
Button.Size = ButtonSize;

export default Button;
