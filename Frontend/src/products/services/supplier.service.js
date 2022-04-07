import axios from 'axios'

export default class SupplierService {
    api = '/suppliers';

    getSuppliers() {
        return axios.get(this.api)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    getSupplierById(id) {
        return axios.get(`${this.api}/${id}`)
            .then(response => response.data)
            .catch(error => console.error(error));
    }
}