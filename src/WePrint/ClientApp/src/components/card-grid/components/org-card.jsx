import React from 'react';
import PropTypes from 'prop-types';
import { useHistory } from 'react-router-dom';

import OrgApi from '../../../api/OrganizationApi';
import './org-card.scss';

function OrgCard(props) {
  const { orgId, name, link, location, projectCount } = props;
  const history = useHistory();
  return (
    <div
      className="org-card"
      onClick={() => {
        history.push(link);
      }}
    >
      <div className="org-card__info">
        <img className="org-card__icon" src={OrgApi.getAvatarUrl(orgId)} alt="Organization Logo" />
        <div className="org-card__title">{name}</div>
      </div>
      <div className="org-card__detail">
        <span>{location}</span>
        <span>{projectCount}</span>
      </div>
    </div>
  );
}

OrgCard.propTypes = {
  orgId: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  link: PropTypes.string.isRequired,
  location: PropTypes.string.isRequired,
  projectCount: PropTypes.string.isRequired,
};

export default OrgCard;
