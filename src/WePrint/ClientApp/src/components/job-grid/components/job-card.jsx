import React from 'react';
import PropTypes from 'prop-types';
import JobPlaceholder from '../../../assets/images/job.png';
import './job-card.scss';

function JobCard(props) {
  const { image } = props;

  return (
    <div className="job-card">
      <img className="job-card__image" src={image} alt="Job" />
      <div className="job-card__body"></div>
    </div>
  );
}

JobCard.propTypes = {
  image: PropTypes.string.isRequired,
};

JobCard.defaultProps = {
  image: JobPlaceholder,
};

export default JobCard;
