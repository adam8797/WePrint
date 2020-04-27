import React, { useState } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import { WepInput, WepNumber, Button } from '../../../components';
import './create-pledge.scss';
import ProjectApi from '../../../api/ProjectApi';

function CreatePledge({ projId }) {
  const [units, setUnits] = useState('0');
  const [delivery, setDelivery] = useState('');
  const [anon, setAnon] = useState(false);

  const submitPledge = () => {
    ProjectApi.pledgesFor(projId)
      .add({
        deliveryDate: moment(delivery).toJSON(),
        quantity: +units,
        anonymous: anon,
      })
      .subscribe(
        pledge => console.log('pledge', pledge),
        err => console.error(err)
      );
  };

  return (
    <div className="pledge-form">
      <div className="input-group">
        <label htmlFor="units">Units</label>
        <WepNumber
          name="units"
          id="units"
          value={units}
          handleChange={ev => setUnits(ev.target.value)}
        />
      </div>
      <div className="input-group">
        <label htmlFor="delivery">Estimated Delivery</label>
        <WepInput
          name="delivery"
          id="delivery"
          value={delivery}
          handleChange={ev => setDelivery(ev.target.value)}
        />
      </div>
      <div className="input-group input-group--inline">
        <input
          type="checkbox"
          name="anon"
          id="anon"
          value={anon}
          onChange={ev => setAnon(ev.target.checked)}
        />
        <label htmlFor="anon">I would like to remain anonymous</label>
      </div>
      <div className="input-group">
        <Button type={Button.Type.SUCCESS} onClick={submitPledge}>
          Make Pledge
        </Button>
      </div>
    </div>
  );
}

CreatePledge.propTypes = {
  projId: PropTypes.string.isRequired,
};

export default CreatePledge;
