export class AddressModel {

    constructor(streetAddress, city, state, zipCode){
        this.streetAdress = streetAddress;
        this.city = city;
        this.state = state;
        this.zipCode = zipCode;
    }

    static Equals(a, b)
    {
        if(a === b && a === null) return true;
        if(a !== null || b!== null) return false;
        if(a === b && a === undefined) return true;
        if(a !== undefined || b !== undefined) return false;
        return a.streetAdress === b.streetAdress && 
            a.city === b.city &&
            a.state === b.state &&
            a.zipCode == b.zipCode;
    }
}