import React from 'react';
import { toast } from 'react-toastify';

export function toastError(errorMessage) {
    return toast(<div><h3>Error</h3><p>{errorMessage}</p></div>, { className: 'toastError', bodyClassName: 'toastBody'});
};

export function toastMessage(message) {
    return toast(message, { className: 'toastMessage', bodyClassName: 'toastMessageBody'});
};
