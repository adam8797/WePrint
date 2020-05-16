import React from 'react';
import PropTypes from 'prop-types';
import { Progress } from 'reactstrap';
import filesize from 'filesize';
import { BodyCard, Button, FileDrop, Table } from '../../../components';

const fileUploadCols = [
  {
    id: 'delete',
    accessor: 'fileName',
    // eslint-disable-next-line react/prop-types
    Cell: ({ cell: { value: fileName } }) => (
      <Button
        icon="trash"
        size={Button.Size.SMALL}
        type={Button.Type.DANGER}
        onClick={() => this.removeFile(fileName)}
      />
    ),
  },
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
    Header: 'Progress',
    accessor: 'progress',
    // eslint-disable-next-line react/prop-types
    Cell: ({ cell: { value: progress } }) => (
      <Progress color={progress.color} value={progress.percent}>
        {progress.label}
      </Progress>
    ),
  },
];

function StageUpload({
  name,
  files,
  maxFiles,
  uploadComplete,
  handleFileChange,
  reverseAction,
  advanceAction,
}) {
  // map state.files to proper table data and exclude excess
  const fileUploadData = files.map(({ fileName, size, progress }) => ({
    fileName,
    size,
    progress,
  }));

  let advanceValid = true;
  let tooltip;
  if (!files.length) {
    advanceValid = false;
    tooltip = 'Please upload at least one file';
  } else if (uploadComplete) {
    advanceValid = false;
    tooltip = 'Upload currently in progress';
  }

  return (
    <BodyCard centered className="post-job-page">
      <h2>Upload Files to: {name}</h2>
      <FileDrop
        className="post-job-page__file-drop"
        handleFiles={handleFileChange}
        accept=".3mf, .stl"
        multiple
        disabled={files.length >= maxFiles}
        customMsg="Drag .3mf & .stl files here, or click to select files"
        disabledMsg={`Cannot add more files, max ${maxFiles} allowed`}
      />
      <div className="file-list">
        <Table columns={fileUploadCols} data={fileUploadData} emptyMessage="No files added yet" />
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
          disabled={!advanceValid}
          tooltip={tooltip}
        >
          Next
        </Button>
      </div>
    </BodyCard>
  );
}

StageUpload.propTypes = {
  name: PropTypes.string,
  files: PropTypes.arrayOf(
    PropTypes.shape({
      fileName: PropTypes.string,
      size: PropTypes.number,
      fileData: PropTypes.object,
      progress: PropTypes.shape({
        color: PropTypes.string,
        label: PropTypes.string,
        percent: PropTypes.number,
      }),
    })
  ),
  maxFiles: PropTypes.number,
  uploadComplete: PropTypes.bool,
  handleFileChange: PropTypes.func,
  reverseAction: PropTypes.func,
  advanceAction: PropTypes.func,
};

export default StageUpload;
