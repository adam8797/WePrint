import React, { Component } from 'react';
import PropTypes from 'prop-types';
import PrinterApi from '../../../api/PrinterApi';
import BidApi from '../../../api/BidApi';
import SectionTitle from '../../../components/section-title/section-title';
import Button from '../../../components/button/button';
import { WepNumber, WepDropdown, WepTextarea } from '../../../components';
import { MaterialType, MaterialColor, FinishType } from '../../../models/Enums';
import BidModel from '../../../models/BidModel';
import './bid-post.scss';

function makeOpts(obj) {
  return Object.entries(obj).map(([key, value]) => ({
    displayName: value,
    value: key,
  }));
}

class BidPost extends Component {
  constructor(props) {
    super(props);
    this.state = {
      printers: [],
      showForm: false,
      submitted: props.submitted,
      price: null,
      printer: '',
      supportDensity: null,
      shellThickness: null,
      layerHeight: null,
      fill: null,
      material: '',
      color: '',
      finishing: '',
      turnAround: null,
      notes: '',
    };
  }

  componentDidMount() {
    PrinterApi.getAll().subscribe(printers => {
      this.setState({ printers });
    });
  }

  postBid = () => {
    const {
      price,
      printer,
      supportDensity,
      shellThickness,
      layerHeight,
      fill,
      material,
      color,
      finishing,
      turnAround,
      notes,
    } = this.state;
    const { bidderId, jobId } = this.props;

    const bid = new BidModel();
    bid.id = 0;
    bid.bidderId = bidderId;
    bid.jobId = jobId;
    bid.price = parseFloat(price);
    bid.notes = notes;
    bid.layerHeight = parseFloat(layerHeight);
    bid.shellThickness = parseFloat(shellThickness);
    bid.fillPercentage = parseFloat(fill);
    bid.supportDensity = parseFloat(supportDensity);
    bid.printerId = printer;
    bid.materialType = material;
    bid.materialColor = color;
    bid.finishType = finishing;
    bid.workTime = `${turnAround}.00:00:00`;
    BidApi.CreateBid(bid, jobId).subscribe(() => {
      this.setState({ submitted: true });
    });
  };

  handleFormChange = ev => {
    const { name, value } = ev.target;
    this.setState({ [name]: value });
  };

  getContent = () => {
    const { submitted } = this.state;
    if (submitted) {
      return (
        <div className="bid-post__form-content">You have already submitted a bid for this job</div>
      );
    }

    const {
      printers,
      price,
      printer,
      supportDensity,
      shellThickness,
      layerHeight,
      fill,
      material,
      color,
      finishing,
      turnAround,
      notes,
    } = this.state;

    const formValid =
      price &&
      printer &&
      supportDensity &&
      shellThickness &&
      layerHeight &&
      fill &&
      material &&
      color &&
      finishing &&
      turnAround;

    return (
      <div className="bid-post__form-content">
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="price">
            Price ($)
          </label>
          <WepNumber
            name="price"
            id="price"
            value={price}
            placeholder="00"
            min={1}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="printer">
            Printer
          </label>
          <WepDropdown
            name="printer"
            id="printer"
            value={printer}
            placeholder="Printer..."
            options={printers.map(p => ({ value: p.id, displayName: p.name }))}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="supportDensity">
            Support Density
          </label>
          <WepNumber
            name="supportDensity"
            id="supportDensity"
            value={supportDensity}
            placeholder="00"
            min={1}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="shellThickness">
            Shell Thickness (mm)
          </label>
          <WepNumber
            name="shellThickness"
            id="shellThickness"
            value={shellThickness}
            placeholder="00"
            min={1}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="layerHeight">
            Layer Height (mm)
          </label>
          <WepNumber
            name="layerHeight"
            id="layerHeight"
            value={layerHeight}
            placeholder="00"
            min={1}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="fill">
            Fill Percentage (%)
          </label>
          <WepNumber
            name="fill"
            id="fill"
            value={fill}
            placeholder="00"
            min={1}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="material">
            Material Type
          </label>
          <WepDropdown
            name="material"
            id="material"
            value={material}
            placeholder="Material Type..."
            options={makeOpts(MaterialType)}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="color">
            Material Color
          </label>
          <WepDropdown
            name="color"
            id="color"
            value={color}
            placeholder="Material Color..."
            options={makeOpts(MaterialColor)}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="finishing">
            Finishing
          </label>
          <WepDropdown
            name="finishing"
            id="finishing"
            value={finishing}
            placeholder="Finishing..."
            options={makeOpts(FinishType)}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="turnAround">
            Turnaround Time (days)
          </label>
          <WepNumber
            name="turnAround"
            id="turnAround"
            value={turnAround}
            placeholder="00"
            min={0}
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input">
          <label className="bid-post__label" htmlFor="notes">
            Other Notes
          </label>
          <WepTextarea
            name="notes"
            id="notes"
            value={notes}
            placeholder="Other notes..."
            handleChange={this.handleFormChange}
          />
        </div>
        <div className="bid-post__input bid-post__input--space">
          <Button
            size={Button.Size.SMALL}
            type={Button.Type.DANGER}
            onClick={() => {
              this.setState({ showForm: false });
            }}
          >
            Cancel
          </Button>
          <Button
            size={Button.Size.SMALL}
            onClick={this.postBid}
            type={Button.Type.SUCCESS}
            disabled={!formValid}
          >
            Submit
          </Button>
        </div>
      </div>
    );
  };

  render() {
    const { showForm } = this.state;
    return (
      <div>
        <SectionTitle title="Post a Bid" />
        {showForm ? (
          <div className="bid-post">{this.getContent()}</div>
        ) : (
          <div className="bid-post__input bid-post__input--center">
            <Button
              size={Button.Size.SMALL}
              onClick={() => {
                this.setState({ showForm: true });
              }}
            >
              Post a Bid
            </Button>
          </div>
        )}
      </div>
    );
  }
}

BidPost.propTypes = {
  submitted: PropTypes.bool.isRequired,
  jobId: PropTypes.string.isRequired,
  bidderId: PropTypes.string.isRequired,
};

export default BidPost;
