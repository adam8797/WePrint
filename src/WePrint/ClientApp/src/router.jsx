import React from 'react';
import PropTypes from 'prop-types';
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import {
  AppLayout,
  About,
  Devices,
  FindJob,
  Help,
  Home,
  PageNotFound,
  PostJob,
  Topics,
  JobDetail,
} from './views';

export default function AppRouter({ basename }) {
  return (
    <BrowserRouter basename={basename}>
      <AppLayout>
        <Switch>
          <Route path="/about">
            <About />
          </Route>
          <Route path="/help">
            <Help />
          </Route>
          <Route path="/devices">
            <Devices />
          </Route>
          <Route path="/find">
            <FindJob />
          </Route>
          <Route path="/post">
            <PostJob />
          </Route>
          <Route path="/topics">
            <Topics />
          </Route>
          <Route exact path="/">
            <Home />
          </Route>
          <Route path="/job/:jobId">
            <JobDetail />
          </Route>
          <Route path="*">
            <PageNotFound />
          </Route>
        </Switch>
      </AppLayout>
    </BrowserRouter>
  );
}

AppRouter.propTypes = {
  basename: PropTypes.string.isRequired,
};
