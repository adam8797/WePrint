import React from 'react';
import './search-item.scss'

class SearchItem extends React.Component {

  generateLink(item) {
    return item.href;
  }

  render() {
    return (
      <div className="search-item">
        { this.props.item.type === "Organization" && <img src={this.props.item.imageUrl} className="avatar"/> }
        <a href={this.generateLink(this.props.item)}><h2>{this.props.item.title}</h2></a>
        <p>{this.props.item.description}</p>
      </div>
    );
  }
}

export default SearchItem;