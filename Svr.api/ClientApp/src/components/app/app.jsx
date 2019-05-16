import React from 'react';
import { Route } from 'react-router-dom';
import { HomePage, ClaimPage, Layout } from '../pages';

import './app.css';

export default () => (
  <Layout>
    <Route exact path="/" component={HomePage} />
    <Route path="/claims" component={ClaimPage} />
  </Layout>
);

