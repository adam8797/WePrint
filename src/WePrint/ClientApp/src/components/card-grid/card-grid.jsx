import React from 'react';
import PropTypes from 'prop-types';
import JobCard from './components/job-card';
import OrgCard from './components/org-card';
import ProjCard from './components/proj-card';

import './card-grid.scss';

export const CardTypes = {
  JOB: 'JobCard',
  ORGANIZATION: 'OrgCard',
  PROJECT: 'ProjCard',
};

function CardGrid(props) {
  const { cards, cardType } = props;

  function getGridFooter() {
    if (!cards.length) {
      return <div className="card-grid__content--empty">No Data To Display</div>;
    }
    return (
      <div className="card-grid__content-count">
        Showing <strong>{cards.length}</strong> Result{cards.length !== 1 && 's'}
      </div>
    );
  }

  function getCard(card) {
    switch (cardType) {
      case CardTypes.JOB:
        return (
          <JobCard
            name={card.name}
            link={card.link}
            image={card.image}
            customerUserName={card.customerUserName}
            customerId={card.customerId}
            parts={card.parts}
            printTime={card.printTime}
            prints={card.prints}
            source={card.source}
            externalId={card.externalId}
            status={card.status}
            key={card.id}
          />
        );
      case CardTypes.ORGANIZATION:
        return (
          <OrgCard
            name={card.name}
            orgId={card.id}
            link={card.link}
            location={card.location}
            projectCount={card.projectCount}
            key={card.id}
          />
        );
      case CardTypes.PROJECT:
        return (
          <ProjCard
            projId={card.id}
            title={card.title}
            location={card.location}
            goal={card.goal}
            closed={card.closed}
            progressDisplay={card.progressDisplay}
            link={card.link}
            key={card.id}
          />
        );
      default:
        console.error(`Card type ${cardType} not recognized`);
        return '';
    }
  }

  return (
    <div className="card-grid">
      <div className="card-grid__content">{cards.map(card => getCard(card))}</div>
      <div className="card-grid__footer">{getGridFooter()}</div>
    </div>
  );
}

CardGrid.propTypes = {
  cards: PropTypes.arrayOf(
    PropTypes.objectOf(
      PropTypes.oneOfType([
        PropTypes.string,
        PropTypes.number,
        PropTypes.object,
        PropTypes.array,
        PropTypes.bool,
      ])
    )
  ),
  cardType: PropTypes.string,
};

CardGrid.CardTypes = CardTypes;

export default CardGrid;
