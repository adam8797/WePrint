import React, { Component } from 'react';
import { Table } from '../../../components';
import './job.scss';

class Job extends Component {
  getSampleData() {
    this.title = "Emily's Job";
    this.poster = 'Emily';
    this.biddingStatus = 'OPEN';
    this.statusStyle = 'open';
    this.bidDeadline = '02/20/2020';
    this.bidDeadlineStyle = 'close';
    this.timeLeft = '1 day';
    this.printerType = 'SLA';
    this.material = 'Red Resin';
    this.destination = 'Philadelphia';
    this.image = 'http://placekitten.com/250';
    this.description = `
      Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec fringilla ultrices egestas. Nam porttitor mauris enim, sagittis tincidunt justo tempus sed. Quisque enim nisi, tincidunt eget velit id, faucibus semper libero. Curabitur ex ipsum, posuere vitae dolor a, maximus imperdiet sem. Mauris volutpat urna sed lacus luctus, semper pharetra ipsum pellentesque. Mauris augue est, semper eu tincidunt eu, viverra a sem. Donec purus urna, pretium ac turpis eget, tincidunt porta elit. Mauris accumsan libero dui, nec hendrerit nisl dapibus sit amet. Nulla sed lacus neque. Proin at ante maximus, accumsan mauris id, sagittis arcu. Aenean semper justo ac augue luctus, vitae euismod neque mattis. Donec viverra quis elit ac egestas.
    `;
    this.filesTable = {
      columns: [
        {
          Header: 'Part Name',
          accessor: 'part',
        },
        {
          Header: 'File Size',
          accessor: 'size',
        },
        {
          Header: 'Dimensions',
          accessor: 'dimensions',
        },
        {
          Header: 'Filament Usage',
          accessor: 'filamentUsage',
        },
        {
          Header: 'Time Estimate',
          accessor: 'timeEst',
        },
      ],
      data: [
        {
          part: 'Part 1',
          size: '200 MB',
          dimensions: '100mm x 100 mm x 25 mm',
          filamentUsage: '20,000 mm',
          timeEst: '2 Hours',
        },
        {
          part: 'Part 2',
          size: '400 MB',
          dimensions: '150mm x 150 mm x 50 mm',
          filamentUsage: '25,000 mm',
          timeEst: '4 Hours',
        },
      ],
    };
    this.bidTable = {
      columns: [
        {
          Header: 'Bid',
          accessor: 'bid',
        },
        {
          Header: 'User',
          accessor: 'user',
        },
        {
          Header: 'Printer',
          accessor: 'printer',
        },
        {
          Header: 'Material',
          accessor: 'material',
        },
        {
          Header: 'Color',
          accessor: 'color',
        },
        {
          Header: 'Estimate', // time accepted to time it's put in the box
          accessor: 'estimate',
        },
        {
          Header: 'User Rating',
          accessor: 'rating',
        },
      ],
      data: [
        {
          bid: '$5',
          user: 'Steve',
          printer: 'Boring Printer',
          material: '3D Print Material',
          color: 'Red',
          estimate: '7 days',
          rating: '3 stars',
        },
        {
          bid: '$100',
          user: 'Mike',
          printer: 'Fancy Printer',
          material: 'Fancy Material',
          color: 'Rainbow',
          estimate: '2 days',
          rating: '5 stars',
        },
      ],
    };
  }

  fetchJob() {
    // TODO: replace this with an api call to fetch the data we need
    this.getSampleData();
  }

  render() {
    this.fetchJob();
    return (
      <div className="job">
        <div className="job__header">
          <span className="job__title">{this.title}</span>
          <span className="job__subtitle">
            <span>Posted by: {this.poster}</span>
            <h4>
              Bidding:{' '}
              <span className={`job__status--${this.statusStyle}`}>{this.biddingStatus}</span>
            </h4>
          </span>
          <hr />
        </div>
        <div className="job__body">
          <img className="job__image" src={this.image} alt="Job Preview" />
          <div className="job__detail">
            <span>
              <span className="job__section">Bid Deadline:</span>
              {this.bidDeadline}
              <span className={`job__deadline--${this.bidDeadlineStyle}`}>
                &nbsp; ({this.timeLeft} left)
              </span>
            </span>
            <span>
              <span className="job__section">Printer Type:</span>
              {this.printerType}
            </span>
            <span>
              <span className="job__section">Material:</span>
              {this.material}
            </span>
            <span>
              <span className="job__section">Destination:</span>
              {this.destination}
            </span>
            <span>
              <span className="job__section">Description:</span>
              <br />
              <span className="job__description">{this.description}</span>
            </span>
          </div>
        </div>
        <div className="job__bids">
          <Table title="Bids" columns={this.bidTable.columns} data={this.bidTable.data} />
        </div>
        <div className="job__files">
          <Table title="Files" columns={this.filesTable.columns} data={this.filesTable.data} />
        </div>
      </div>
    );
  }
}

export default Job;
