import React from 'react';
import PropTypes from 'prop-types';
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import axios from 'axios';
import {
  AppLayout,
  About,
  EditDevice,
  Devices,
  Find,
  Help,
  Home,
  PageNotFound,
  PostJob,
  JobDetail,
  FinishedJobs,
  Organization,
  EditOrganization,
  Project,
} from './views';

export default function AppRouter({ basename }) {
  return (
    <BrowserRouter basename={basename}>
      <AppLayout>
        <Switch>
          <Route
            path="/login"
            component={() => {
              window.location.href = `${window.location.origin}/Identity/Account/Login`;
              return null;
            }}
          />
          <Route
            path="/logout"
            component={() => {
              axios.get(`${window.location.origin}/api/auth/logout`).then(() => {
                // force a refresh cause our app currently doesn't have
                // global state to update the header and it doesn't retry to load user itself
                window.location.href = `${window.location.origin}/`;
              });
              return null;
            }}
          />
          <Route path="/about">
            <About />
          </Route>
          <Route path="/help">
            <Help />
          </Route>
          <Route path="/edit-device/:printerId?">
            <EditDevice />
          </Route>
          <Route path="/devices">
            <Devices />
          </Route>
          <Route path="/find">
            <Find />
          </Route>
          <Route path="/post">
            <PostJob />
          </Route>
          <Route path="/finished">
            <FinishedJobs />
          </Route>
          <Route exact path="/">
            <Home />
          </Route>
          <Route path="/job/:jobId">
            <JobDetail />
          </Route>
          <Route path="/organization/:orgId">
            <Organization />
          </Route>
          <Route path="/new-organization">
            <EditOrganization
              returnCallback={org => {
                let loc = window.location.origin;
                if (org && org.id) {
                  loc += `/organization/${org.id}`;
                }
                window.location.href = loc;
              }}
            />
          <Route path="/project/:projId">
            <Project />
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
