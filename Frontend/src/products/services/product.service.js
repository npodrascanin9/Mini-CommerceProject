import axios from 'axios'

export default class ProductService {
    // #region APIs
    api = '/products';
    orderDetailApi = 'orderDetails'
    // #endregion

    // #region Product Services
    getProducts() {
        return axios.get(this.api)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    getById(id) {
        return axios.get(`${this.api}/${id}`)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    createProduct(input) {
        return axios.post(this.api, input)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    updateProduct(id, input) {
        return axios.put(`${this.api}/${id}`, input)
            .then(() => {})
            .catch(error => console.error(error));
    }

    deleteProductById(id) {
        return axios.delete(`${this.api}/${id}`)
            .then(() => {})
            .catch(error => console.error(error));
    }

    // #endregion

    // #region OrderDetails Service
    getOrderDetailsByProductId(productId) {
        return axios.get(`${this.api}/${productId}/${this.orderDetailApi}`)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    checkIfOrderDetailExists(productId, id) {
        return axios.get(`${this.api}/${productId}/${this.orderDetailApi}/${id}/exists`)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    getOrderDetailByProductIdAndId(productId, id) {
        return axios.get(`${this.api}/${productId}/${this.orderDetailApi}/${id}`)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    createOrderDetailByProductId(productId, input) {
        return axios.post(`${this.api}/${productId}/${this.orderDetailApi}`, input)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    updateOrderDetailByProductIdAndId(productId, id, input) {
        return axios.put(`${this.api}/${productId}/${this.orderDetailApi}/${id}`, input)
            .then(() => { })
            .catch(error => console.error(error));
    }

    deleteOrderDetailByProductIdAndId(productId, id) {
        return axios.delete(`${this.api}/${productId}/${this.orderDetailApi}/${id}`)
            .then(() => { })
            .catch(error => console.error(error));
    }
    // #endregion
}