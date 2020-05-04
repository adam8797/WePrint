import React from 'react';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import './status-view.scss';

function StatusView(props) {
  const { text, icon, spin, size = '1x' } = props;
  return (
    <div className="status-view">
      <div className="status-view__text">{text}</div>
      {icon && <FontAwesomeIcon icon={icon} spin={spin} size={size} />}
    </div>
  );
}

StatusView.propTypes = {
  text: PropTypes.string.isRequired,
  icon: PropTypes.oneOfType([PropTypes.string, PropTypes.arrayOf(PropTypes.string)]),
  spin: PropTypes.bool,
  size: PropTypes.string,
};

export default StatusView;
