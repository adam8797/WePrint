import React from 'react';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import './account-restricted-view.scss';

export const ViewSize = {
  SMALL: 'small',
  STANDARD: 'standard',
};

function AccountRestrictedView(props) {
  const { text, size } = props;
  return (
    <div className={`account-restricted account-restricted--${size}`}>
      <FontAwesomeIcon className="account-restricted__icon" icon="user-lock" />
      <div className="account-restricted__text">{text}</div>
      <div className="account-restricted__sub-text">
        Click <Link to="/login">here</Link> to be redirected to the login screen
      </div>
    </div>
  );
}

AccountRestrictedView.propTypes = {
  text: PropTypes.string,
  size: PropTypes.string,
};

AccountRestrictedView.defaultProps = {
  text: 'You must be logged in to view this page.',
  size: ViewSize.STANDARD,
};

AccountRestrictedView.Size = ViewSize;

export default AccountRestrictedView;
