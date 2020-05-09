import React from 'react';
import { toast } from 'react-toastify';

export function toastError(errorMessage, opts) {
  return toast(
    <div>
      <h3>Error</h3>
      <p>{errorMessage}</p>
    </div>,
    { className: 'toastError', bodyClassName: 'toastBody', ...opts }
  );
}

export function toastMessage(message, opts) {
  return toast(message, {
    className: 'toastMessage',
    bodyClassName: 'toastMessageBody',
    autoClose: 5000,
    ...opts,
  });
}
