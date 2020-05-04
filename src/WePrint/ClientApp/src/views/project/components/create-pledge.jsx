import React, { useState } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import { WepInput, WepNumber, Button, WepModal, toastMessage } from '../../../components';
import './create-pledge.scss';
import ProjectApi from '../../../api/ProjectApi';

function CreatePledge({ projId, modalOpen, closeModal }) {
  const [units, setUnits] = useState('');
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
        pledge => {
          console.log('pledge', pledge);
          toastMessage('Pledge has been submitted successfully!');
          closeModal();
        },
        err => console.error(err)
      );
  };

  return (
    <WepModal isOpen={modalOpen} onRequestClose={closeModal} contentLabel="Create Pledge">
      <h2>Make a Pledge</h2>
      <div className="pledge-form">
        <div className="input-group">
          <label htmlFor="units">Units*</label>
          <WepNumber
            name="units"
            id="units"
            value={units}
            placeholder="00"
            handleChange={ev => setUnits(ev.target.value)}
          />
        </div>
        <div className="input-group">
          <label htmlFor="delivery">Estimated Delivery Date*</label>
          <WepInput
            name="delivery"
            id="delivery"
            value={delivery}
            placeholder={moment().format('MM/DD/YYYY')}
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
          <WepModal.ButtonContainer>
            <Button type={Button.Type.DANGER} onClick={closeModal}>
              Cancel
            </Button>
            <Button type={Button.Type.SUCCESS} onClick={submitPledge}>
              Make Pledge
            </Button>
          </WepModal.ButtonContainer>
        </div>
      </div>
    </WepModal>
  );
}

CreatePledge.propTypes = {
  projId: PropTypes.string.isRequired,
  modalOpen: PropTypes.bool.isRequired,
  closeModal: PropTypes.func.isRequired,
};

export default CreatePledge;
