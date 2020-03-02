import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import AppRouter from './router';
import './assets/styles/_base.scss';
import { BidApi } from './api/BidApi';
import { ArrayDeepEquals } from './api/CommonApi';
import { BidModel } from './models/BidModel';
// import registerServiceWorker from './registerServiceWorker';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(<AppRouter basename={baseUrl} />, rootElement);

// Uncomment the line above that imports the registerServiceWorker function
// and the line below to register the generated service worker.
// By default create-react-app includes a service worker to improve the
// performance of the application by caching static assets. This service
// worker can interfere with the Identity UI, so it is
// disabled by default when Identity is being used.
//
// registerServiceWorker();