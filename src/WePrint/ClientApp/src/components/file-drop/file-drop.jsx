import React, { useCallback } from 'react';
import PropTypes from 'prop-types';
import { useDropzone } from 'react-dropzone';
import classNames from 'classnames';
import './file-drop.scss';

const noop = () => {};

function FileDrop({
  className,
  multiple,
  disabled,
  customMsg,
  dropMsg,
  disabledMsg,
  accept,
  handleFiles = noop,
}) {
  const onDrop = useCallback(
    acceptedFiles => {
      // Do something with the files
      handleFiles(acceptedFiles);
    },
    [handleFiles]
  );
  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    disabled,
    accept,
  });

  const fileDropClass = classNames('file-drop', className, {
    'file-drop--disabled': disabled,
    'file-drop--active': isDragActive,
  });

  let message = customMsg || 'Drag your files here, or click to select files';
  if (isDragActive) message = dropMsg || 'Drop the files here...';
  if (disabled) message = disabledMsg || 'File input disabled';

  return (
    <div
      {...getRootProps({
        className: fileDropClass,
      })}
    >
      <input {...getInputProps({ multiple })} />
      <p>{message}</p>
    </div>
  );
}

FileDrop.propTypes = {
  className: PropTypes.string,
  customMsg: PropTypes.string,
  dropMsg: PropTypes.string,
  disabledMsg: PropTypes.string,
  multiple: PropTypes.bool,
  disabled: PropTypes.bool,
  // comma separated list of accepted types
  // See here for details https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file#Unique_file_type_specifiers
  accept: PropTypes.string,
  handleFiles: PropTypes.func,
};

export default FileDrop;
