import React from 'react';
import PropTypes from 'prop-types';
import filesize from 'filesize';
import { BodyCard, Button, Table } from '../../../components';

const fileProcessCols = [
  {
    Header: 'Filename',
    accessor: 'fileName',
  },
  {
    Header: 'Size',
    accessor: 'size',
    Cell: ({ cell: { value } }) => filesize(value),
  },
  {
    Header: 'Volume',
    accessor: 'volume',
    Cell: ({ cell: { value } }) => value || 'Pending...',
  },
  {
    Header: 'Est. Time',
    accessor: 'estTime',
    Cell: ({ cell: { value } }) => value || 'Pending...',
  },
];

function StageProcess({ files, sliceData, reverseAction, advanceAction }) {
  const fileProcessData = files.map(({ fileName, size }) => {
    let volume = 'Processing...';
    let estTime = 'Processing...';
    if (sliceData) {
      const i = sliceData.findIndex(sData => sData.fileName === fileName);
      if (i !== -1) {
        const { matEstimates, timeEstimates } = sliceData[i];
        volume = matEstimates.map(matEst => `${matEst.value}(${matEst.unit})`).join(' x ');
        estTime =
          timeEstimates.reduce((acc, timeEst) => acc + timeEst.timeSpan, 0) / timeEstimates.length;
      }
    }
    return {
      fileName,
      size,
      volume,
      estTime,
    };
  });

  return (
    <BodyCard centered className="post-job-page">
      <h2>Your job has been created!</h2>
      <p>
        We&apos;re currently processing your files. You can wait for processing to finish or you can
        submit now.
      </p>
      <div className="file-list-processing">
        <Table
          columns={fileProcessCols}
          data={fileProcessData}
          emptyMessage="Something went wrong, this job has no files!"
        />
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
        >
          Done
        </Button>
      </div>
    </BodyCard>
  );
}

StageProcess.propTypes = {
  files: PropTypes.arrayOf(
    PropTypes.shape({
      fileName: PropTypes.string,
      size: PropTypes.number,
      fileData: PropTypes.object,
      progress: PropTypes.object,
    })
  ),
  sliceData: PropTypes.arrayOf(
    PropTypes.objectOf(PropTypes.oneOf([PropTypes.string, PropTypes.object]))
  ),
  reverseAction: PropTypes.func,
  advanceAction: PropTypes.func,
};

export default StageProcess;
