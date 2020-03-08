import React from 'react';
import PropTypes from 'prop-types';
import { MaterialColor, MaterialType } from '../../../models/Enums';
import { BodyCard, Button, WepInput, WepDropdown, WepTextarea } from '../../../components';

const biddingOpts = [
  { displayName: '1 Day', value: '1' },
  { displayName: '2 Days', value: '2' },
  { displayName: '3 Days', value: '3' },
  { displayName: '4 Days', value: '4' },
  { displayName: '5 Days', value: '5' },
];
const matTypeOpts = Object.entries(MaterialType).map(([key, value]) => ({
  displayName: value,
  value: key,
}));
const matColorOpts = Object.entries(MaterialColor).map(([key, value]) => ({
  displayName: value,
  value: key,
}));

function StageInfo({
  name,
  biddingPeriod,
  materialType,
  materialColor,
  description,
  handleFormChange,
  reverseAction,
  advanceAction,
}) {
  // these are the only required fields
  const jobFormValid = name && biddingPeriod && materialType && materialColor;

  return (
    <BodyCard centered className="post-job-page">
      <h2>Basic Info</h2>
      <div className="basic-info__form">
        <div className="input-group">
          <label htmlFor="name">Job Name:</label>
          <WepInput
            name="name"
            id="name"
            value={name}
            placeholder="Job name..."
            handleChange={handleFormChange}
          />
        </div>
        <div className="input-group">
          <label htmlFor="biddingPeriod">Bidding Period:</label>
          <WepDropdown
            name="biddingPeriod"
            id="biddingPeriod"
            value={biddingPeriod}
            placeholder="Bidding Period..."
            options={biddingOpts}
            handleChange={handleFormChange}
          />
        </div>
        <div className="input-group">
          <label htmlFor="materialType">Material Type:</label>
          <WepDropdown
            name="materialType"
            id="materialType"
            value={materialType}
            placeholder="Select one..."
            options={matTypeOpts}
            handleChange={handleFormChange}
          />
        </div>
        <div className="input-group">
          <label htmlFor="materialColor">Material Color:</label>
          <WepDropdown
            name="materialColor"
            id="materialColor"
            value={materialColor}
            placeholder="Select one..."
            options={matColorOpts}
            handleChange={handleFormChange}
          />
        </div>
        <div className="input-group input-group--wide">
          <label htmlFor="description">Description:</label>
          <WepTextarea
            name="description"
            id="description"
            value={description}
            placeholder="Description here"
            handleChange={handleFormChange}
          />
        </div>
      </div>
      <div className="body-card__actions">
        <Button
          type={Button.Type.DANGER}
          className="body-card__action-left"
          onClick={reverseAction}
        >
          Back
        </Button>
        <Button
          type={Button.Type.SUCCESS}
          className="body-card__action-right"
          onClick={advanceAction}
          disabled={!jobFormValid}
        >
          Next
        </Button>
      </div>
    </BodyCard>
  );
}

StageInfo.propTypes = {
  name: PropTypes.string,
  biddingPeriod: PropTypes.oneOf(['', ...biddingOpts.map(opt => opt.value)]),
  materialType: PropTypes.oneOf(['', ...matTypeOpts.map(opt => opt.value)]),
  materialColor: PropTypes.oneOf(['', ...matColorOpts.map(opt => opt.value)]),
  description: PropTypes.string,
  handleFormChange: PropTypes.func,
  reverseAction: PropTypes.func,
  advanceAction: PropTypes.func,
};

export default StageInfo;
