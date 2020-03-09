import React, { useState, useEffect } from 'react';
import { useHistory, useParams } from 'react-router-dom';
import { PrinterType } from '../../models/Enums';
import PrinterModel from '../../models/PrinterModel';
import PrinterApi from '../../api/PrinterApi';
import { BodyCard, Button, SectionTitle, WepInput, WepNumber, WepTextarea } from '../../components';
import './edit-device.scss';

function EditDevice() {
  const { printerId } = useParams();
  const [name, setName] = useState('');
  const [printerType, setPrinterType] = useState('');
  const [layerMin, setLayerMin] = useState('');
  const [dimensions, setDimensions] = useState({ x: '', y: '', z: '' });
  const [description, setDescription] = useState('');

  const history = useHistory();

  useEffect(() => {
    // This should work, relies on the API changing printer ids to not use `/`
    if (printerId) {
      PrinterApi.GetPrinter(printerId).subscribe(printer => {
        setName(printer.name);
        setPrinterType(printer.type);
        setLayerMin(printer.layerMin);
        setDimensions({
          x: printer.xMax,
          y: printer.yMax,
          z: printer.zMax,
        });
        setDescription(printer.description);
      });
    }
  }, [printerId]);

  const handleDimensionChange = ev => {
    const { name: inputName, value } = ev.target;
    setDimensions({
      ...dimensions,
      [inputName]: value,
    });
  };

  const handleSubmit = () => {
    const printer = new PrinterModel();
    printer.name = name;
    printer.type = printerType;
    printer.xMax = dimensions.x;
    printer.yMax = dimensions.y;
    printer.zMax = dimensions.z;
    printer.layerMin = layerMin;

    if (printerId) {
      // update not create
      PrinterApi.UpdatePrinter(printerId, printer).subscribe({
        error: console.error,
        complete: () => {
          history.push('/devices');
        },
      });
    } else {
      PrinterApi.CreatePrinter(printer).subscribe({
        error: console.error,
        complete: () => {
          history.push('/devices');
        },
      });
    }
  };

  const handleDelete = () => {
    if (printerId) {
      // check again just for safety
      // TODO: add better confirmation alert
      // eslint-disable-next-line no-alert
      if (window.confirm(`Do you really want to delete printer: ${name}?`)) {
        PrinterApi.DeletePrinter(printerId).subscribe({
          error: console.error,
          complete: () => {
            history.push('/devices');
          },
        });
      }
    }
  };

  // TODO: Fix poor man's form validation
  const formValid =
    name && printerType && layerMin > 0 && dimensions.x > 0 && dimensions.y > 0 && dimensions.z > 0;

  return (
    <BodyCard centered className="add-device-page">
      <h2>{printerId ? 'Edit' : 'New'} Device</h2>

      <div className="add-device-page__form">
        <div className="input-group">
          <label htmlFor="name">Name:</label>
          <WepInput
            name="name"
            id="name"
            value={name}
            placeholder="Printer Name..."
            handleChange={ev => setName(ev.target.value)}
          />
        </div>
        <SectionTitle title="Device Type" />
        <div className="type-buttons">
          {/* TODO: duplicate of job workflow stage one, consider extracting */}
          <Button
            type={printerType === PrinterType.FDM ? Button.Type.SUCCESS : Button.Type.PRIMARY}
            size={Button.Size.LARGE}
            onClick={() => setPrinterType(PrinterType.FDM)}
          >
            FDM
          </Button>
          <Button
            type={printerType === PrinterType.SLA ? Button.Type.SUCCESS : Button.Type.PRIMARY}
            size={Button.Size.LARGE}
            onClick={() => setPrinterType(PrinterType.SLA)}
          >
            SLA
          </Button>
          <Button
            type={printerType === PrinterType.LaserCut ? Button.Type.SUCCESS : Button.Type.PRIMARY}
            size={Button.Size.LARGE}
            onClick={() => setPrinterType(PrinterType.LaserCut)}
          >
            Laser
          </Button>
        </div>
        <SectionTitle title="Device Details" />
        <div className="add-device-page__details">
          <div className="input-group">
            <label htmlFor="x">Maximum Dimensions:</label>
            <div className="add-device-page__dimensions">
              <label htmlFor="x">x</label>
              <WepNumber
                name="x"
                id="x"
                value={dimensions.x}
                min={1}
                placeholder="00"
                handleChange={handleDimensionChange}
              />
              <label htmlFor="y">y</label>
              <WepNumber
                name="y"
                id="y"
                value={dimensions.y}
                min={1}
                placeholder="00"
                handleChange={handleDimensionChange}
              />
              <label htmlFor="z">z</label>
              <WepNumber
                name="z"
                id="z"
                value={dimensions.z}
                min={1}
                placeholder="00"
                handleChange={handleDimensionChange}
              />
            </div>
          </div>
          <div className="input-group">
            <label htmlFor="name">Minimum Layer Height:</label>
            <WepNumber
              name="layerMin"
              id="layerMin"
              value={layerMin}
              min={1}
              placeholder="00"
              handleChange={ev => setLayerMin(ev.target.value)}
            />
          </div>
          <div className="input-group">
            <label htmlFor="name">Model:</label>
            <WepTextarea
              name="description"
              id="description"
              value={description}
              placeholder="Personal printer description..."
              handleChange={ev => setDescription(ev.target.value)}
            />
          </div>
        </div>
      </div>

      <div className="body-card__actions">
        {printerId && (
          <Button
            type={Button.Type.DANGER}
            className="body-card__action-left"
            onClick={handleDelete}
          >
            Delete
          </Button>
        )}
        <Button
          type={Button.Type.SUCCESS}
          className="body-card__action-right"
          onClick={handleSubmit}
          disabled={!formValid}
        >
          Save
        </Button>
      </div>
    </BodyCard>
  );
}

export default EditDevice;
