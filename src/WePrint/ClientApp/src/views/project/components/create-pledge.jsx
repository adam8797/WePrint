import React from 'react';
import { useForm } from 'react-hook-form';
import PropTypes from 'prop-types';
import moment from 'moment';
import { useHistory } from 'react-router-dom';
import { isEmpty } from 'lodash';
import { WepInput, WepNumber, Button, WepModal, toastMessage } from '../../../components';
import './create-pledge.scss';
import ProjectApi from '../../../api/ProjectApi';

function CreatePledge({ projId, modalOpen, closeModal }) {
  const history = useHistory();
  const { register, handleSubmit, errors } = useForm();

  const submitPledge = form => {
    const { units, anon, delivery } = form;
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
          history.push(`/project/${pledge.project}/pledge/${pledge.id}`);
        },
        err => console.error(err)
      );
  };

  return (
    <WepModal isOpen={modalOpen} onRequestClose={closeModal} contentLabel="Create Pledge">
      <h2>Make a Pledge</h2>
      <form onSubmit={handleSubmit(submitPledge)}>
        <div className="pledge-form">
          <div className="input-group">
            <label htmlFor="units">Units*</label>
            <WepNumber
              name="units"
              register={register({ required: true, min: 1 })}
              id="units"
              value={0}
              placeholder="00"
              error={!!errors.units}
            />
            {errors.units && (
              <div className="input-group__error">
                Please enter a valid unit value of at least 1
              </div>
            )}
          </div>
          <div className="input-group">
            <label htmlFor="delivery">Estimated Delivery Date*</label>
            <WepInput
              name="delivery"
              id="delivery"
              value=""
              register={register({ required: true, pattern: /[01]\d\/[0123]\d\/[2]\d\d\d/ })}
              placeholder={moment().format('MM/DD/YYYY')}
              error={!!errors.delivery}
            />
            {errors.units && (
              <div className="input-group__error">
                Please enter a valid delivery date in the format MM/DD/YYYY
              </div>
            )}
          </div>
          <div className="input-group input-group--inline">
            <input type="checkbox" name="anon" ref={register} id="anon" defaultValue={false} />
            <label htmlFor="anon">I would like to remain anonymous</label>
          </div>
          <div className="input-group">
            <WepModal.ButtonContainer>
              <Button type={Button.Type.DANGER} onClick={closeModal}>
                Cancel
              </Button>
              <Button type={Button.Type.SUCCESS} htmlType="submit" disabled={!isEmpty(errors)}>
                Make Pledge
              </Button>
            </WepModal.ButtonContainer>
          </div>
        </div>
      </form>
    </WepModal>
  );
}

CreatePledge.propTypes = {
  projId: PropTypes.string.isRequired,
  modalOpen: PropTypes.bool.isRequired,
  closeModal: PropTypes.func.isRequired,
};

export default CreatePledge;
