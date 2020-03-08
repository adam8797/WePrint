import React from 'react';
import PropTypes from 'prop-types';
import { PrinterType } from '../../../models/Enums';
import { BodyCard, Button } from '../../../components';

function StageType({ printerType, setPrinterType, advanceAction }) {
  return (
    <BodyCard centered className="post-job-page">
      <h2>Select Type of Job</h2>
      <div className="type-buttons">
        <Button
          type={printerType === PrinterType.FDM ? Button.Type.SUCCESS : Button.Type.PRIMARY}
          size={Button.Size.LARGE}
          onClick={() => setPrinterType(PrinterType.FDM)}
        >
          FDM
        </Button>
        <Button
          type={printerType === PrinterType.SLA ? Button.Type.SUCCESS : Button.Type.PRIMARY}
          size={Button.Size.LARGE}
          onClick={() => setPrinterType(PrinterType.SLA)}
        >
          SLA
        </Button>
        <Button
          type={printerType === PrinterType.LaserCut ? Button.Type.SUCCESS : Button.Type.PRIMARY}
          size={Button.Size.LARGE}
          onClick={() => setPrinterType(PrinterType.LaserCut)}
        >
          Laser
        </Button>
      </div>
      <div className="body-card__actions">
        <Button
          type={Button.Type.SUCCESS}
          className="body-card__action-right"
          onClick={advanceAction}
          disabled={!printerType}
        >
          Next
        </Button>
      </div>
    </BodyCard>
  );
}

StageType.propTypes = {
  printerType: PropTypes.oneOf(['', ...Object.values(PrinterType)]),
  setPrinterType: PropTypes.func,
  advanceAction: PropTypes.func,
};

export default StageType;
