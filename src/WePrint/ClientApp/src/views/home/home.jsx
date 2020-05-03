import React from 'react';
import { Link } from 'react-router-dom';
import { BodyCard, SectionTitle } from '../../components';
import './home.scss';

const HomePage = () => (
  <BodyCard className="homepage">
    <SectionTitle title="Welcome to WePrint" />
    <p>We&apos;re happy you&apos;re here!</p>

    <SectionTitle title="What We Do" headerSize={4} />
    <p>WePrint is a service to help the 3D Printing community come together to serve a common goal. 
      With the recent Covid-19 outbreak, we&apos;ve seen the 3D printing community unite to help reduce the equipment shortage
      currently being felt by hospitals and other emergency services. Our goal is to make this faster and easier, so that more people
      can contribute towards this effort.</p>
    <p>WePrint allows organizations to set up 3D printing projects for the benefit of institutions near them. For example, an organization
      might ask for face masks for a local hospital. Users with 3D printers can search for these projects and pledge parts for the cause.</p>

    <SectionTitle title="Get Started" headerSize={4} />
    <p>Feel free to click the <Link to="/find">Find a Job</Link> link to look for projects in your area. Ready to contribute?
      Login or make an account by clicking the <Link to="/login">Login button</Link> in the top right.</p>
  </BodyCard>
);

export default HomePage;
