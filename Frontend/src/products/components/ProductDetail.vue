<template>
  <div>
    <Toast />
    <!-- #region Product Detail form -->
    <div class="product-detail">
      <BlockUI :blocked="loading" :fullScreen="true"></BlockUI>
      <ProgressBar v-if="loading" mode="indeterminate" style="height: 0.5em" />
      <Toolbar style="margin-bottom: 15px">
        <template #start>
          <Button
            :label="isNew ? 'Insert' : 'Edit'"
            @click="onSubmit"
            icon="pi pi-save"
            class="p-ml-2"
          />
        </template>
        <template #end>
          <Button
            label="Products"
            icon="pi pi-table"
            class="p-button-secondary"
            @click="onProducts"
          />
        </template>
      </Toolbar>

      <Card>
        <template #content>
          <div class="p-fluid p-formgrid p-grid">
            <div class="p-field p-col-12 p-md-6">
              <label for="productName">Product Name</label>
              <InputText
                id="productName"
                type="text"
                v-model.trim="form.productName.value"
              />
              <p v-if="!form.productName.isValid" class="error">
                Product name is required
              </p>
            </div>
            <div class="p-field p-col-12 p-md-6">
              <label for="supplier">Supplier</label>
              <Dropdown
                v-model="form.supplier.value"
                :options="suppliers"
                optionLabel="contactName"
                placeholder="Select Supplier"
                :filter="true"
                filterPlaceholder="Search Supplier"
              />
              <p v-if="!form.supplier.isValid" class="error">
                Supplier is required
              </p>
            </div>
            <div class="p-field p-col-12 p-md-6">
              <label for="category">Category</label>
              <Dropdown
                v-model="form.category.value"
                :options="categories"
                optionLabel="categoryName"
                placeholder="Select Category"
                :filter="true"
                filterPlaceholder="Search Category"
                :disabled="form.category.disabled"
              />
            </div>
            <div class="p-field p-col-12 p-md-6">
              <label for="quantityPerUnit">Quantity per unit</label>
              <InputText
                id="productName"
                type="text"
                name="quantityPerUnit"
                v-model.trim="form.quantityPerUnit.value"
              />
              <p v-if="!form.quantityPerUnit.isValid" class="error">
                Quantity per unit is required
              </p>
            </div>
            <div class="p-field p-col-12 p-md-6">
              <label for="unitPrice">Unit Price</label>
              <InputText
                id="unitPrice"
                type="number"
                name="unitPrice"
                v-model.number="form.unitPrice.value"
              />
              <p v-if="!form.unitPrice.isValid" class="error">
                Unit price is required and must be between 0 and 1000
              </p>
            </div>
            <div class="p-field p-col-12 p-md-6">
              <label for="unitsInStock">Units In Stock</label>
              <InputText
                id="unitsInStock"
                type="number"
                name="unitsInStock"
                v-model.number="form.unitsInStock.value"
              />
              <p v-if="!form.unitsInStock.isValid" class="error">
                Units in stock is required and must be between 0 and 1000
              </p>
            </div>

            <div class="p-field p-col-12 p-md-6">
              <label for="unitsOnOrder">Units on order</label>
              <InputText
                id="unitsOnOrder"
                type="number"
                name="unitsOnOrder"
                v-model.number="form.unitsOnOrder.value"
              />
              <p v-if="!form.unitsOnOrder.isValid" class="error">
                Units on order is required and must be between 0 and 1000
              </p>
            </div>

            <div class="p-field p-col-12 p-md-6">
              <label for="reorderLevel">Reorder Level</label>
              <InputText
                id="reorderLevel"
                name="reorderLevel"
                type="number"
                v-model.number="form.reorderLevel.value"
              />
              <p v-if="!form.reorderLevel.isValid" class="error">
                Reorder Level is required and must be between 0 and 1000
              </p>
            </div>
          </div>
        </template>

        <template #footer>
          <div class="p-field-checkbox">
            <Checkbox
              id="discontinued"
              v-model="form.discontinued.value"
              :binary="true"
            />
            <label for="discontinued">Discontinued</label>
          </div>
        </template>
      </Card>
    </div>
    <!-- #endregion -->

    <!-- #region OrderDetail List -->
    <div
      v-if="!isNew"
      class="orderdetail-list"
      style="margin-top: 50px; margin-bottom: 40px"
    >
      <Toolbar>
        <template #start>
          <h1>Order Detail List</h1>
        </template>
        <template #end>
          <Button
            icon="pi pi-plus"
            class="p-button-success"
            @click="onAddOrderDetail"
          />
        </template>
      </Toolbar>

      <DataTable
        :value="orderDetails"
        responsiveLayout="scroll"
        :scrollable="true"
        style="width: 100%"
        sortMode="multiple"
        :paginator="true"
        :rows="10"
        :loading="loading"
        :rowsPerPageOptions="[10, 20, 50]"
        dataKey="orderId"
        selectionMode="single"
        @row-select="onRowOrderDetailSelect"
        class="p-datatable-customers card-border"
      >
        <Column
          v-for="col of orderDetailCols"
          :field="col.field"
          :header="col.header"
          :key="col.field"
        ></Column>
      </DataTable>
    </div>
    <!-- #endregion -->

    <!-- #region Order Detail Form dialog -->
    <Dialog
      :visible="orderDetailDialog.display"
      :modal="true"
      :closable="false"
      :style="{ width: '50vw' }"
    >
      <template #header>
        <h1 class="p-text-left">
          {{
            orderDetailDialog.isNew ? "Add order detail" : "Edit order detail"
          }}
        </h1>
      </template>

      <div class="order-detail-form">
        <div class="p-fluid p-formgrid p-grid">
          <div class="p-field p-col-12">
            <label for="order">Order</label>
            <Dropdown
              v-model="orderDetailDialog.form.order.value"
              :options="orders"
              optionLabel="shipName"
              placeholder="Select Order"
              :filter="true"
              filterPlaceholder="Search Order"
              :disabled="!orderDetailDialog.isNew"
            />
            <p v-if="!orderDetailDialog.form.order.isValid" class="error">
              Order is required
            </p>
          </div>

          <div class="p-field p-col-12 p-md-6">
            <label for="product">Product</label>
            <Dropdown
              v-model="orderDetailDialog.form.product.value"
              :options="products"
              optionLabel="productName"
              placeholder="Select Product"
              :filter="true"
              :disabled="orderDetailDialog.form.product.disabled"
              filterPlaceholder="Search Product"
            />
            <p v-if="!orderDetailDialog.form.product.isValid" class="error">
              Product is required
            </p>
          </div>

          <div class="p-field p-col-12 p-md-6">
            <label for="discount">Discount</label>
            <InputText
              id="discount"
              type="number"
              name="discount"
              v-model.number="orderDetailDialog.form.discount.value"
            />
            <p v-if="!orderDetailDialog.form.discount.isValid" class="error">
              Discount is required and must be between 0 and 1
            </p>
          </div>

          <div class="p-field p-col-12 p-md-6">
            <label for="quantity">Quantity</label>
            <InputText
              id="quantity"
              type="number"
              name="quantity"
              v-model.number="orderDetailDialog.form.quantity.value"
            />
            <p v-if="!orderDetailDialog.form.quantity.isValid" class="error">
              Quantity is required and must be between 0 and 1000
            </p>
          </div>

          <div class="p-field p-col-12 p-md-6">
            <label for="unitPrice">Unit Price</label>
            <InputText
              id="unitPrice"
              name="unitPrice"
              type="number"
              v-model.number="orderDetailDialog.form.unitPrice.value"
            />
            <p v-if="!orderDetailDialog.form.unitPrice.isValid" class="error">
              Unit price is required and must be between 0 and 1000
            </p>
          </div>
        </div>

        <BlockUI
          :blocked="orderDetailDialog.loading"
          :fullScreen="true"
        ></BlockUI>
        <ProgressBar
          v-if="orderDetailDialog.loading"
          mode="indeterminate"
          style="height: 0.5em"
        />
        <Toolbar style="margin-top: 20px">
          <template #start>
            <Button
              :label="orderDetailDialog.isNew ? 'Insert' : 'Edit'"
              @click="onOrderDetailSubmit"
              icon="pi pi-save"
              class="p-ml-2"
            />
            <Button
              v-if="!orderDetailDialog.isNew"
              label="Delete"
              @click="onDeleteOrderDetail"
              icon="pi pi-trash"
              class="p-ml-2 p-button-danger"
            />
          </template>
          <template #end>
            <Button
              icon="pi pi-times"
              class="p-button-secondary p-mr-2"
              label="Cancel"
              @click="onCloseOrderDetailDialog()"
            />
          </template>
        </Toolbar>
      </div>
    </Dialog>
    <!-- #endregion -->
  </div>
</template>

<script>
import Toolbar from "primevue/toolbar";
import Button from "primevue/button";
import Card from "primevue/card";
import InputText from "primevue/inputtext";
import Dropdown from "primevue/dropdown";
import Checkbox from "primevue/checkbox";
import BlockUI from "primevue/blockui";
import ProgressBar from "primevue/progressbar";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Dialog from "primevue/dialog";
import Toast from "primevue/toast";

import ProductService from "../services/product.service";
import CategoryService from "../services/category.service";
import SupplierService from "../services/supplier.service";
import OrderService from "../services/order.service";

export default {
  components: {
    Toolbar,
    Button,
    Card,
    InputText,
    Dropdown,
    Checkbox,
    BlockUI,
    ProgressBar,
    DataTable,
    Column,
    Dialog,
    Toast,
  },
  data() {
    return {
      isNew: this.$route.params.id === "new",
      id: null,
      categoryId: Number(this.$route.params.categoryId),
      productService: new ProductService(),
      supplierService: new SupplierService(),
      categoryService: new CategoryService(),
      orderService: new OrderService(),
      loading: true,
      input: {},
      form: {
        isValidForm: true,
        productName: {
          value: "",
          isValid: true,
        },
        supplier: {
          value: "",
          isValid: true,
        },
        category: {
          value: "",
          isValid: true,
          disabled: true,
        },
        quantityPerUnit: {
          value: "",
          isValid: true,
        },
        unitPrice: {
          value: "",
          isValid: true,
        },
        unitsInStock: {
          value: "",
          isValid: true,
        },
        unitsOnOrder: {
          value: "",
          isValid: true,
        },
        reorderLevel: {
          value: "",
          isValid: true,
        },
        discontinued: {
          value: false,
          isValid: true,
        },
      },
      categories: [],
      suppliers: [],
      products: [],
      orders: [],
      orderDetails: [],
      orderDetailCols: [
        { field: "discount", header: "Discount" },
        { field: "quantity", header: "Quantity" },
        { field: "unitPrice", header: "UnitPrice" },
        { field: "productName", header: "Product Name" },
        { field: "shipName", header: "Ship Name" },
      ],
      orderDetailDialog: {
        isNew: this.$route.params.id === "new",
        display: false,
        loading: false,
        header: "",
        orderId: null,
        form: {
          isValidForm: true,
          discount: {
            value: "",
            isValid: true,
          },
          quantity: {
            value: "",
            isValid: true,
          },
          unitPrice: {
            value: "",
            isValid: true,
          },
          product: {
            value: "",
            isValid: true,
            disabled: true,
          },
          order: {
            value: "",
            isValid: true,
            disabled: true,
          },
        },
      },
    };
  },
  created() {
    this.categoryService
      .getCategories()
      .then((response) => {
        this.categories = response;
        this.form.category.value = this.categories.find(
          (c) => c.categoryId === this.categoryId
        );
      })
      .catch((error) => console.error(error));

    this.supplierService
      .getSuppliers()
      .then((response) => (this.suppliers = response))
      .catch((error) => console.error(error));

    this.productService
      .getProducts()
      .then((response) => (this.products = response))
      .catch((error) => console.error(error))
      .finally(() => {
        console.log(this.products);
      });

    this.orderService
      .getOrders()
      .then((response) => (this.orders = response))
      .catch((error) => console.error(error))
      .finally(() => {
        console.log(this.orders);
      });
  },
  mounted() {
    this.id = this.isNew ? null : Number(this.$route.params.id);
    if (!this.isNew) {
      this.initData();
      this.onLoadOrderDetailsData();
      this.orderDetailDialog.header = "Edit order detail";
    } else {
      this.orderDetailDialog.header = "Add order detail";
      this.loading = false;
    }
  },
  methods: {
    initData() {
      this.productService
        .getById(this.id)
        .then((response) => {
          this.form.productName.value = response.productName;
          this.form.quantityPerUnit.value = response.quantityPerUnit;
          this.form.unitPrice.value = response.unitPrice;
          this.form.unitsInStock.value = response.unitsInStock;
          this.form.unitsOnOrder.value = response.unitsOnOrder;
          this.form.reorderLevel.value = response.reorderLevel;
          this.form.discontinued.value = response.discontinued;
          this.form.supplier.value = this.suppliers.find(
            (s) => s.supplierId == response.supplierId
          );
          this.form.category.value = this.categories.find(
            (c) => c.categoryId == response.categoryId
          );
        })
        .catch((error) => console.error(error))
        .finally(() => {
          this.loading = false;
        });
    },
    onValidate() {
      this.form.productName.isValid = this.form.productName.value
        ? true
        : false;
      this.form.supplier.isValid = this.form.supplier.value ? true : false;
      this.form.quantityPerUnit.isValid = this.form.quantityPerUnit.value
        ? true
        : false;

      if (this.form.unitPrice.value) {
        this.form.unitPrice.isValid =
          this.form.unitPrice.value >= 0 && this.form.unitPrice.value <= 1000;
      } else {
        this.form.unitPrice.isValid = false;
      }

      if (this.form.unitsInStock.value) {
        this.form.unitsInStock.isValid =
          this.form.unitsInStock.value >= 0 &&
          this.form.unitsInStock.value <= 1000;
      } else {
        this.form.unitsInStock.isValid = false;
      }

      if (this.form.unitsOnOrder.value) {
        this.form.unitsOnOrder.isValid =
          this.form.unitsOnOrder.value >= 0 &&
          this.form.unitsOnOrder.value <= 1000;
      } else {
        this.form.unitsOnOrder.isValid = false;
      }

      if (this.form.reorderLevel.value) {
        this.form.reorderLevel.isValid =
          this.form.reorderLevel.value >= 0 &&
          this.form.reorderLevel.value <= 1000;
      } else {
        this.form.reorderLevel.isValid = false;
      }

      this.form.isValidForm =
        this.form.productName.isValid &&
        this.form.supplier.isValid &&
        this.form.quantityPerUnit.isValid &&
        this.form.unitPrice.isValid &&
        this.form.unitsInStock.isValid &&
        this.form.unitsOnOrder.isValid &&
        this.form.reorderLevel.isValid;
    },
    onSubmit() {
      this.onValidate();
      if (!this.form.isValidForm) {
        return;
      }
      console.log(this.form);
      this.initInputDto();
      if (this.isNew) {
        this.productService
          .createProduct(this.input)
          .then((response) => {
            console.log(response);
            this.$toast.add({severity:'success', summary: `Create with id='${response.productId}'`, detail:'Success', life: 3000});
          })
          .catch((error) => {
            console.error(error);
            this.$toast.add({
              severity: "error",
              summary: "Error",
              detail: `${error.data.response}`,
              life: 3000,
            });
          })
          .finally(() => {
            this.$router.push(
              `/categories/${this.categoryId}/products`
            );
          });
      } else {
        this.productService
          .updateProduct(this.id, this.input)
          .then(() => {
            this.$toast.add({severity:'success', summary: `Updated product`, detail:'Success', life: 3000});
          })
          .catch((error) => {
            console.error(error);
            this.$toast.add({
              severity: "error",
              summary: "Error",
              detail: `${error.data.response}`,
              life: 3000,
            });
          })
          .finally(() => {});
      }
    },
    onProducts() {
      this.$router.push(`/categories/${this.categoryId}/products`);
    },
    initInputDto() {
      this.input = {
        productName: this.form.productName.value,
        quantityPerUnit: this.form.quantityPerUnit.value,
        unitPrice: this.form.unitPrice.value,
        unitsInStock: this.form.unitsInStock.value,
        unitsOnOrder: this.form.unitsOnOrder.value,
        reorderLevel: this.form.reorderLevel.value,
        discontinued: this.form.discontinued.value,
        supplierId: this.form.supplier.value?.supplierId,
        categoryId: this.form.category.value?.categoryId,
      };

      if (!this.isNew) {
        this.input.productId = Number(this.id);
      } else {
        this.input.createOrderDetails = this.orderDetails;
      }

      console.log(this.input);
    },
    onLoadOrderDetailsData() {
      this.loading = true;
      this.productService
        .getOrderDetailsByProductId(this.id)
        .then((response) => (this.orderDetails = response))
        .catch((error) => console.error(error))
        .finally(() => {
          this.loading = false;
        });
    },
    onDeleteOrderDetail() {
      if (this.isNew) {
        return;
      }

      this.productService
        .deleteOrderDetailByProductIdAndId(
          this.id,
          this.orderDetailDialog.orderId
        )
        .then(() => {
          this.$toast.add({
            severity: "success",
            summary: `Deleted with id='${this.orderDetailDialog.orderId}`,
            detail: "Success",
            life: 3000,
          });
        })
        .catch((error) => console.error(error))
        .finally(() => {
          this.onLoadOrderDetailsData();
          this.orderDetailDialog.display = false;
        });
    },
    onRowOrderDetailSelect(event) {
      if (this.isNew) {
        return;
      }
      this.orderDetailDialog.loading = true;
      this.orderDetailDialog.orderId = event.data.orderId;
      this.productService
        .getOrderDetailByProductIdAndId(this.id, event.data.orderId)
        .then((response) => {
          console.log(response);
          this.orderDetailDialog.form.discount.value = response.discount;
          this.orderDetailDialog.form.quantity.value = response.quantity;
          this.orderDetailDialog.form.unitPrice.value = response.unitPrice;
          this.orderDetailDialog.form.product.value = this.products.find(
            (p) => p.productId == response.productId
          );
          this.orderDetailDialog.form.order.value = this.orders.find(
            (o) => o.orderId == response.orderId
          );
        })
        .catch((error) => console.error(error))
        .finally(() => {
          this.orderDetailDialog.isNew = false;
          this.orderDetailDialog.display = true;
          this.orderDetailDialog.loading = false;
          this.loading = false;
        });
    },
    onAddOrderDetail() {
      this.orderDetailDialog.isNew = true;
      this.onResetOrderDetailForm();
      this.orderDetailDialog.display = true;
    },
    onOrderDetailValidate() {
      this.orderDetailDialog.form.order.isValid = this.orderDetailDialog.form
        .order.value
        ? true
        : false;
      this.orderDetailDialog.form.product.isValid = this.orderDetailDialog.form
        .product.value
        ? true
        : false;

      if (this.orderDetailDialog.form.quantity.value) {
        this.orderDetailDialog.form.quantity.isValid =
          this.orderDetailDialog.form.quantity.value >= 0 &&
          this.orderDetailDialog.form.quantity.value <= 1000;
      } else {
        this.orderDetailDialog.form.quantity.isValid = false;
      }

      if (
        this.orderDetailDialog.form.discount.value ||
        this.orderDetailDialog.form.discount.value == 0
      ) {
        this.orderDetailDialog.form.discount.isValid =
          this.orderDetailDialog.form.discount.value >= 0 &&
          this.orderDetailDialog.form.discount.value <= 1;
      } else {
        this.orderDetailDialog.form.discount.isValid = false;
      }

      if (this.orderDetailDialog.form.unitPrice.value) {
        this.orderDetailDialog.form.unitPrice.isValid =
          this.orderDetailDialog.form.unitPrice.value >= 0 &&
          this.orderDetailDialog.form.unitPrice.value <= 1000;
      } else {
        this.orderDetailDialog.form.unitPrice.isValid = false;
      }

      this.orderDetailDialog.form.isValidForm =
        this.orderDetailDialog.form.product.isValid &&
        this.orderDetailDialog.form.order.isValid &&
        this.orderDetailDialog.form.quantity.isValid &&
        this.orderDetailDialog.form.discount.isValid &&
        this.orderDetailDialog.form.unitPrice.isValid;
    },
    onOrderDetailSubmit() {
      this.loading = true;
      if (this.isNew) {
        this.loading = false;
        return;
      }

      this.onOrderDetailValidate();
      if (!this.orderDetailDialog.form.isValidForm) {
        this.loading = false;
        return;
      }

      this.initInputOrderDetailDto();

      var existsOrderDetail = false;
      this.productService.checkIfOrderDetailExists(this.id, this.orderDetailDialog.input.orderId)
        .then(response => {
          existsOrderDetail = response;
        })
        .catch(error => console.error(error))
        .finally(() => {

        })

      if (this.orderDetailDialog.isNew) {
        if (existsOrderDetail) {
          this.$toast.add({severity:'error', summary: 'Error', detail:'Selected order with selected product already exists!', life: 3000});
          return;
        }

        this.productService
          .createOrderDetailByProductId(this.id, this.orderDetailDialog.input)
          .then((response) => {
            this.$toast.add({severity:'success', summary: 'Success', detail:'Order detail added', life: 3000});
            console.log(response);
            this.onLoadOrderDetailsData();
          })
          .catch((error) => console.error(error))
          .finally(() => {
            this.orderDetailDialog.display = false;
            this.loading = false;
          });
      } else {
        this.productService
          .updateOrderDetailByProductIdAndId(
            this.id,
            this.orderDetailDialog.input.orderId,
            this.orderDetailDialog.input
          )
          .then(() => {
            this.$toast.add({severity:'success', summary: 'Success', detail:`Order detail updated`, life: 3000});
            this.onLoadOrderDetailsData();
          })
          .catch((error) => console.error(error))
          .finally(() => {
            this.orderDetailDialog.display = false;
            this.loading = false;
          });
      }
    },
    initInputOrderDetailDto() {
      this.orderDetailDialog.input = {
        orderId: this.orderDetailDialog.form.order.value?.orderId,
        discount: this.orderDetailDialog.form.discount.value,
        quantity: this.orderDetailDialog.form.quantity.value,
        unitPrice: this.orderDetailDialog.form.unitPrice.value,
        product: this.isNew ? "" : this.orderDetailDialog.form.product.value,
      };
    },
    onCloseOrderDetailDialog() {
      this.orderDetailDialog.display = false;
    },
    onResetOrderDetailForm() {
      this.orderDetailDialog.form.discount.value = "";
      this.orderDetailDialog.form.quantity.value = "";
      this.orderDetailDialog.form.unitPrice.value = "";
      this.orderDetailDialog.form.product.value = this.products.find(
        (p) => p.productId == this.id
      );
      this.orderDetailDialog.form.order.value = "";
    },
  },
};
</script>

<style scoped>
.error {
  color: #f00;
  font-weight: bold;
}
</style>
