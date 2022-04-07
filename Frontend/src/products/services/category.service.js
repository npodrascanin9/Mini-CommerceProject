import axios from 'axios';

export default class CategoryService {
    // #region APIs
    api = '/categories';
    apiProducts = 'products';
    // #endregion

    // #region Category Service
    getCategories() {
        return axios.get(this.api)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    getCategoryById(id) {
        return axios.get(`${this.api}/${id}`)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    createCategory(input) {
        return axios.post(this.api, input)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    updateCategory(id, input) {
        return axios.put(`${this.api}/${id}`, input)
            .then(() => {})
            .catch(error => console.error(error));
    }

    uploadImage() {
        console.log('upload image');
    }
    // #endregion

    // #region Product Services
    getProductsByCategoryId(categoryId, filter) {
        console.log(`filter: ${filter}`);
        return axios.get(`${this.api}/${categoryId}/${this.apiProducts}`, 
            { 
                params: filter
            })
            .then(response => response.data)
            .catch(error => console.error(error));
    }
    // #endregion
}