import React from 'react';
import PropTypes from 'prop-types';
import { useHistory } from 'react-router-dom';
import { BodyCard, Button } from '../../../components';

function StageSuccess({ jobId }) {
  const history = useHistory();

  const goToJobPage = () => {
    history.push(`/job/${jobId}`);
  };

  return (
    <BodyCard centered>
      <h2>Success</h2>
      <p>Your job has been posted and is awaiting bids</p>
      {/* TODO: change this into an actual link */}
      <Button type={Button.Type.SUCCESS} onClick={goToJobPage}>
        View Job
      </Button>
    </BodyCard>
  );
}

StageSuccess.propTypes = {
  jobId: PropTypes.string.isRequired,
};

export default StageSuccess;
