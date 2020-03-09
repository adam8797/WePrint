import React from 'react';
import PropTypes from 'prop-types';
import { Link, useRouteMatch } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import './nav-item.scss';

export default function NavItem({ icon, to, children }) {
  const match = useRouteMatch(to);
  let isActive = false;
  if (match) {
    isActive = match.path === '/' ? match.isExact : match.path.startsWith(to);
  }
  return (
    <div className={`nav-item ${isActive ? ' nav-item--active' : ''}`}>
      <Link to={to} className="nav-item__link">
        <div className="nav-item__icon">
          <FontAwesomeIcon icon={icon} />
        </div>

        <div className="nav-item__title">{children}</div>
      </Link>
    </div>
  );
}

NavItem.propTypes = {
  icon: PropTypes.string,
  to: PropTypes.string.isRequired,
  children: PropTypes.string.isRequired,
};

NavItem.defaultProps = {
  icon: '',
};
