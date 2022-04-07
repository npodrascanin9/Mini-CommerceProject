import axios from 'axios';

export default class OrderService {
    getOrders() {
        return axios.get('/orders')
            .then(response => response.data)
            .catch(error => console.error(error));
    }
}