import axios from 'axios';

export default class CategoryService {
    // #region APIs
    api = '/categories';
    // #endregion

    // #region Category Service
    getCategories() {
        return axios.get(this.api)
            .then(response => response.data)
            .catch(error => {
                console.error(error);
                if (error?.response?.data) {
                    alert(error.response.data); // error message
                }
            });
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

    uploadImage(files) {
        return axios.post(`${this.api}/upload`, files)
            .then(response => response.data)
            .catch(error => console.error(error));
    }

    deleteCategoryById(id) {
        return axios.delete(`${this.api}/${id}`)
            .then(() => {})
            .catch(error => console.error(error));
    }
    // #endregion
}