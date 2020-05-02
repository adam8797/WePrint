import React from 'react';
import PropTypes from 'prop-types';
import { useHistory } from 'react-router-dom';

import ProjectApi from '../../../api/ProjectApi';
import './proj-card.scss';

function ProjCard(props) {
  const { projId, title, link, location, goal, closed, progressDisplay } = props;
  const history = useHistory();
  return (
    <div
      className={`proj-card ${closed ? 'proj-card--closed' : ''}`}
      onClick={() => {
        history.push(link);
      }}
    >
      <div className="proj-card__info">
        <img
          className="proj-card__thumbnail"
          src={ProjectApi.getThumbnailUrl(projId)}
          alt="Project Thumbnail"
        />
        <div className="proj-card__title">{title}</div>
      </div>
      <div className="proj-card__detail">
        <div className="proj-card__inline">
          <span>{location}</span>
          <span>Goal: {goal} Units</span>
        </div>
        <div className="proj-card__inline">
          <span className="proj-card__status">{closed ? 'Closed' : 'Open'}</span>
          <span>{progressDisplay}</span>
        </div>
      </div>
    </div>
  );
}

ProjCard.propTypes = {
  projId: PropTypes.string.isRequired,
  title: PropTypes.string.isRequired,
  link: PropTypes.string.isRequired,
  location: PropTypes.string.isRequired,
  goal: PropTypes.number.isRequired,
  closed: PropTypes.bool.isRequired,
  progressDisplay: PropTypes.string.isRequired,
};

export default ProjCard;
