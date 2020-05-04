import React from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import { Link } from 'react-router-dom';
import { Button } from '../../../components';
import PledgeModel from '../../../models/PledgeModel';
import './my-pledges.scss';

function MyPledges({ projId, pledges, openPledgeModal, canPledge }) {
  return (
    <div className="my-pledges">
      <h3 className="my-pledges__title">Your Pledges</h3>
      <div className="my-pledges__list">
        {pledges && pledges.length
          ? pledges.map(pledge => (
              <div className="my-pledges__pledge" key={pledge.id}>
                <div className="my-pledges__pledge-quantity">{pledge.quantity}</div>
                <div className="my-pledges__pledge-created">
                  on{' '}
                  <Link to={`/project/${projId}/pledge/${pledge.id}`}>
                    {moment(pledge.created).format('MMM, Do YYYY')}
                  </Link>
                </div>
                <div className="my-pledges__pledge-status">{pledge.status}</div>
              </div>
            ))
          : "You haven't pledged yet."}
      </div>
      <div className="my-pledges__button">
        <Button type={Button.Type.PRIMARY} onClick={openPledgeModal} disabled={!canPledge}>
          {pledges && pledges.length ? 'Pledge More' : 'Pledge Now!'}
        </Button>
      </div>
    </div>
  );
}

MyPledges.propTypes = {
  projId: PropTypes.string.isRequired,
  pledges: PropTypes.arrayOf(PropTypes.shape(PledgeModel)),
  openPledgeModal: PropTypes.func.isRequired,
  canPledge: PropTypes.bool.isRequired,
};

export default MyPledges;
