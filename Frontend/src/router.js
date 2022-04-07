import VueRouter from 'vue-router';

import NotFound from './assets/layout/NotFound'

// #region components
import CategoryList from './categories/components/CategoryList.vue';
import CategoryDetail from './categories/components/CategoryDetail.vue';
import ProductList from './products/components/ProductList.vue';
import ProductDetail from './products/components/ProductDetail.vue';
// #endregion

const router = new VueRouter({
    routes: [
        { path: '/', redirect: '/categories' },
        { path: '/categories', component: CategoryList },
        { path: '/categories/:id', component: CategoryDetail },
        { path: '/categories/:categoryId/products', component: ProductList },
        { path: '/categories/:categoryId/products/:id', component: ProductDetail },
        { path: '/:notFound(.*)', component: NotFound }
    ]
});

export default router;