import React, { useCallback } from 'react';
import PropTypes from 'prop-types';
import { useDropzone } from 'react-dropzone';
import classNames from 'classnames';
import './file-drop.scss';

const noop = () => {};

function FileDrop({ className, multiple, handleFiles = noop }) {
  const onDrop = useCallback(
    acceptedFiles => {
      // Do something with the files
      handleFiles(acceptedFiles);
    },
    [handleFiles]
  );
  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
  });

  const fileDropClass = classNames('file-drop', className, {
    'file-drop--active': isDragActive,
  });

  return (
    <div
      {...getRootProps({
        className: fileDropClass,
      })}
    >
      <input {...getInputProps({ multiple })} />
      {isDragActive ? (
        <p>Drop the files here ...</p>
      ) : (
        <p>Drag your files here, or click to select files</p>
      )}
    </div>
  );
}

FileDrop.propTypes = {
  className: PropTypes.string,
  multiple: PropTypes.bool,
  handleFiles: PropTypes.func,
};

export default FileDrop;
